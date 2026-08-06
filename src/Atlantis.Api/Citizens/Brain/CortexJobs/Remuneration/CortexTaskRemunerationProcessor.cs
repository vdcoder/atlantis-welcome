using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.Economy.Ledger;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Remuneration;

public sealed class CortexTaskRemunerationProcessor
    : BackgroundService
{
    private static readonly TimeSpan PollInterval =
        TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory
        _scopeFactory;

    private readonly ILogger<
        CortexTaskRemunerationProcessor>
        _logger;

    public CortexTaskRemunerationProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<
            CortexTaskRemunerationProcessor> logger)
    {
        _scopeFactory =
            scopeFactory ??
            throw new ArgumentNullException(
                nameof(scopeFactory));

        _logger =
            logger ??
            throw new ArgumentNullException(
                nameof(logger));
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Cortex task remuneration processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed =
                    await TryProcessOneAsync(
                        stoppingToken);

                if (!processed)
                {
                    await Task.Delay(
                        PollInterval,
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Cortex task remuneration processing " +
                    "failed. The oldest pending remuneration " +
                    "will be retried.");

                await Task.Delay(
                    PollInterval,
                    stoppingToken);
            }
        }
    }

    private async Task<bool> TryProcessOneAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    AtlantisDbContext>();

        var ledgerService =
            scope.ServiceProvider
                .GetRequiredService<
                    LedgerService>();

        var remuneration =
            await dbContext
                .CortexTaskRemunerations
                .Where(
                    entity =>
                        entity.Status ==
                            (int)
                            CortexTaskRemunerationStatus
                                .Pending)
                .OrderBy(
                    entity =>
                        entity.CreatedAt)
                .ThenBy(
                    entity =>
                        entity.Id)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (remuneration is null)
        {
            return false;
        }

        ValidateRemuneration(
            remuneration.Amount,
            remuneration.Currency);

        var referenceId =
            remuneration.Id.ToString("D");

        // Recovery path:
        // payment may have committed before a prior process
        // stopped, but the remuneration row may still say
        // Pending.
        var existingTransactionId =
            await FindExistingLedgerTransactionAsync(
                dbContext,
                remuneration.EmployerAccountId,
                remuneration.Amount,
                remuneration.Currency,
                referenceId,
                cancellationToken);

        if (existingTransactionId.HasValue)
        {
            await MarkPaidAsync(
                dbContext,
                remuneration.Id,
                existingTransactionId.Value,
                cancellationToken);

            _logger.LogInformation(
                "Reconciled Cortex task remuneration " +
                "{RemunerationId} with existing ledger " +
                "transaction {LedgerTransactionId}.",
                remuneration.Id,
                existingTransactionId.Value);

            return true;
        }

        var workerAccountId =
            await ResolveWorkerAccountIdAsync(
                dbContext,
                remuneration.WorkerCitizenId,
                remuneration.Currency,
                cancellationToken);

        _logger.LogInformation(
            "Paying Cortex task remuneration " +
            "{RemunerationId}; task {CortexTaskId}; " +
            "worker {WorkerCitizenId}; amount " +
            "{Amount} {Currency}.",
            remuneration.Id,
            remuneration.CortexTaskId,
            remuneration.WorkerCitizenId,
            remuneration.Amount,
            remuneration.Currency);

        var transfer =
            await ledgerService.TransferAsync(
                fromAccountId:
                    remuneration.EmployerAccountId,

                toAccountId:
                    workerAccountId,

                amount:
                    remuneration.Amount,

                reason:
                    CortexTaskRemunerationLedgerReferences
                        .Reason,

                referenceType:
                    CortexTaskRemunerationLedgerReferences
                        .ReferenceType,

                referenceId:
                    referenceId,

                cancellationToken:
                    cancellationToken);

        await MarkPaidAsync(
            dbContext,
            remuneration.Id,
            transfer.TransactionId,
            cancellationToken);

        _logger.LogInformation(
            "Paid Cortex task remuneration " +
            "{RemunerationId}; ledger transaction " +
            "{LedgerTransactionId}; worker account " +
            "{WorkerAccountId}.",
            remuneration.Id,
            transfer.TransactionId,
            workerAccountId);

        return true;
    }

    private static async Task<string>
        ResolveWorkerAccountIdAsync(
            AtlantisDbContext dbContext,
            string workerCitizenId,
            string currency,
            CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            currency);

        var accounts =
            await dbContext.MoneyAccounts
                .AsNoTracking()
                .Where(
                    account =>
                        account.OwnerId ==
                            workerCitizenId &&

                        account.OwnerType ==
                            (int)
                            AccountOwnerType.Citizen &&

                        account.AccountType ==
                            (int)
                            AccountType.CitizenWallet &&

                        account.Currency ==
                            currency &&

                        account.IsActive)
                .Select(
                    account =>
                        account.Id)
                .ToListAsync(
                    cancellationToken);

        return accounts.Count switch
        {
            1 =>
                accounts[0],

            0 =>
                throw new InvalidOperationException(
                    $"Citizen '{workerCitizenId}' has no " +
                    $"active {currency} citizen wallet."),

            _ =>
                throw new InvalidOperationException(
                    $"Citizen '{workerCitizenId}' has more " +
                    $"than one active {currency} citizen " +
                    "wallet.")
        };
    }

    private static async Task<Guid?>
        FindExistingLedgerTransactionAsync(
            AtlantisDbContext dbContext,
            string employerAccountId,
            decimal amount,
            string currency,
            string referenceId,
            CancellationToken cancellationToken)
    {
        var entries =
            await dbContext.LedgerEntries
                .AsNoTracking()
                .Where(
                    entry =>
                        entry.ReferenceType ==
                            CortexTaskRemunerationLedgerReferences
                                .ReferenceType &&

                        entry.ReferenceId ==
                            referenceId)
                .ToListAsync(
                    cancellationToken);

        if (entries.Count == 0)
        {
            return null;
        }

        if (entries.Count != 2)
        {
            throw new InvalidOperationException(
                $"Remuneration reference '{referenceId}' " +
                $"has {entries.Count} ledger entries; " +
                "exactly two were expected.");
        }

        var transactionIds =
            entries
                .Select(
                    entry =>
                        entry.TransactionId)
                .Distinct()
                .ToList();

        if (transactionIds.Count != 1)
        {
            throw new InvalidOperationException(
                $"Remuneration reference '{referenceId}' " +
                "contains multiple ledger transactions.");
        }

        var debit =
            entries.SingleOrDefault(
                entry =>
                    entry.AccountId ==
                        employerAccountId &&
                    entry.Amount ==
                        -amount);

        var credit =
            entries.SingleOrDefault(
                entry =>
                    entry.AccountId !=
                        employerAccountId &&
                    entry.Amount ==
                        amount);

        if (debit is null ||
            credit is null ||
            debit.Currency != currency ||
            credit.Currency != currency)
        {
            throw new InvalidOperationException(
                $"Remuneration reference '{referenceId}' " +
                "does not contain the expected balanced " +
                "payment entries.");
        }

        return transactionIds[0];
    }

    private static async Task MarkPaidAsync(
        AtlantisDbContext dbContext,
        Guid remunerationId,
        Guid ledgerTransactionId,
        CancellationToken cancellationToken)
    {
        var remuneration =
            await dbContext.CortexTaskRemunerations
                .SingleAsync(
                    entity =>
                        entity.Id ==
                            remunerationId,
                    cancellationToken);

        if (remuneration.Status ==
            (int)CortexTaskRemunerationStatus.Paid)
        {
            if (remuneration.LedgerTransactionId !=
                ledgerTransactionId)
            {
                throw new InvalidOperationException(
                    $"Remuneration '{remunerationId}' is " +
                    "already associated with another ledger " +
                    "transaction.");
            }

            return;
        }

        remuneration.Status =
            (int)
            CortexTaskRemunerationStatus.Paid;

        remuneration.PaidAt =
            DateTimeOffset.UtcNow;

        remuneration.LedgerTransactionId =
            ledgerTransactionId;

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static void ValidateRemuneration(
        decimal amount,
        string currency)
    {
        if (amount <= 0m)
        {
            throw new InvalidOperationException(
                "Cortex task remuneration must be positive.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            currency);
    }
}