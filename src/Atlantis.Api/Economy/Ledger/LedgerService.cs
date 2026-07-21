using Atlantis.Api.Data;
using Atlantis.Api.Economy.Currency;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Economy.Ledger;

public sealed class LedgerService
{
    private readonly AtlantisDbContext _dbContext;
    private readonly TransferSolvencyPolicy _solvencyPolicy;

    public LedgerService(AtlantisDbContext dbContext)
    {
        _dbContext = dbContext;
        _solvencyPolicy = TransferSolvencyPolicy.RejectWhenObservedInsufficient;
    }

    public async Task<decimal> GetBalanceAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        var accountExists = await _dbContext.MoneyAccounts
            .AsNoTracking()
            .AnyAsync(
                account => account.Id == accountId,
                cancellationToken);

        if (!accountExists)
        {
            throw new MoneyAccountNotFoundException(accountId);
        }

        return await _dbContext.LedgerEntries
            .AsNoTracking()
            .Where(entry => entry.AccountId == accountId)
            .SumAsync(
                entry => entry.Amount,
                cancellationToken);
    }

    public async Task<IReadOnlyList<LedgerEntry>> GetRecentEntriesAsync(
        string accountId,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        if (limit < 1 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 100.");
        }

        var accountExists = await _dbContext.MoneyAccounts
            .AsNoTracking()
            .AnyAsync(
                account => account.Id == accountId,
                cancellationToken);

        if (!accountExists)
        {
            throw new MoneyAccountNotFoundException(accountId);
        }

        return await _dbContext.LedgerEntries
            .AsNoTracking()
            .Where(entry => entry.AccountId == accountId)
            .OrderByDescending(entry => entry.CreatedAt)
            .ThenByDescending(entry => entry.Id)
            .Take(limit)
            .Select(entry => new LedgerEntry
            {
                Id = entry.Id,
                TransactionId = entry.TransactionId,
                AccountId = entry.AccountId,
                Amount = entry.Amount,
                Currency = entry.Currency,
                Reason = entry.Reason,
                ReferenceType = entry.ReferenceType,
                ReferenceId = entry.ReferenceId,
                CreatedAt = entry.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<LedgerTransferResult> TransferAsync(
        string fromAccountId,
        string toAccountId,
        decimal amount,
        string reason,
        string? referenceType = null,
        string? referenceId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateTransferRequest(
            fromAccountId,
            toAccountId,
            amount,
            reason);

        amount = AtlantisCurrency.Normalize(amount);

        var accounts = await _dbContext.MoneyAccounts
            .AsNoTracking()
            .Where(account =>
                account.Id == fromAccountId ||
                account.Id == toAccountId)
            .ToListAsync(cancellationToken);

        var fromAccount = accounts
            .SingleOrDefault(account =>
                account.Id == fromAccountId)
            ?? throw new MoneyAccountNotFoundException(
                fromAccountId);

        var toAccount = accounts
            .SingleOrDefault(account =>
                account.Id == toAccountId)
            ?? throw new MoneyAccountNotFoundException(
                toAccountId);

        ValidateAccountsForTransfer(
            fromAccount,
            toAccount);

        await EnsureObservedSolvencyAsync(
            fromAccountId,
            amount,
            cancellationToken);

        return await ExecuteAtomicTransferAsync(
            fromAccount,
            toAccount,
            amount,
            reason,
            referenceType,
            referenceId,
            cancellationToken);
    }

    private static LedgerEntry Map(
        LedgerEntryEntity entity)
    {
        return new LedgerEntry
        {
            Id = entity.Id,
            TransactionId = entity.TransactionId,
            AccountId = entity.AccountId,
            Amount = entity.Amount,
            Currency = entity.Currency,
            Reason = entity.Reason,
            ReferenceType = entity.ReferenceType,
            ReferenceId = entity.ReferenceId,
            CreatedAt = entity.CreatedAt
        };
    }

    private async Task<LedgerTransferResult> ExecuteAtomicTransferAsync(
        MoneyAccountEntity fromAccount,
        MoneyAccountEntity toAccount,
        decimal amount,
        string reason,
        string? referenceType,
        string? referenceId,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var transactionId = Guid.NewGuid();
            var createdAt = DateTimeOffset.UtcNow;

            var debitEntity = new LedgerEntryEntity
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                AccountId = fromAccount.Id,
                Amount = -amount,
                Currency = fromAccount.Currency,
                Reason = reason,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                CreatedAt = createdAt
            };

            var creditEntity = new LedgerEntryEntity
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                AccountId = toAccount.Id,
                Amount = amount,
                Currency = toAccount.Currency,
                Reason = reason,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                CreatedAt = createdAt
            };

            ValidateBalancedEntries(
                debitEntity,
                creditEntity);

            _dbContext.LedgerEntries.AddRange(
                debitEntity,
                creditEntity);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new LedgerTransferResult
            {
                TransactionId = transactionId,
                DebitEntry = Map(debitEntity),
                CreditEntry = Map(creditEntity),
                Amount = amount,
                Currency = fromAccount.Currency
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private static void ValidateTransferRequest(
        string fromAccountId,
        string toAccountId,
        decimal amount,
        string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            fromAccountId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            toAccountId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            reason);

        if (string.Equals(
                fromAccountId,
                toAccountId,
                StringComparison.Ordinal))
        {
            throw new InvalidLedgerTransferException(
                "Source and destination accounts must be different.");
        }

        if (amount <= 0m)
        {
            throw new InvalidLedgerTransferException(
                "Transfer amount must be greater than zero.");
        }

        if (!AtlantisCurrency.IsValidAmount(amount))
        {
            throw new InvalidLedgerTransferException(
                $"Transfer amount must contain no more than " +
                $"{AtlantisCurrency.DecimalPlaces} decimal places.");
        }
    }

    private static void ValidateBalancedEntries(
        LedgerEntryEntity debitEntry,
        LedgerEntryEntity creditEntry)
    {
        if (debitEntry.TransactionId !=
            creditEntry.TransactionId)
        {
            throw new InvalidOperationException(
                "Ledger entries do not share a transaction ID.");
        }

        if (debitEntry.Amount + creditEntry.Amount != 0m)
        {
            throw new InvalidOperationException(
                "Ledger transaction is not balanced.");
        }

        if (!string.Equals(
                debitEntry.Currency,
                creditEntry.Currency,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Ledger transaction contains multiple currencies.");
        }

        if (debitEntry.AccountId ==
            creditEntry.AccountId)
        {
            throw new InvalidOperationException(
                "Ledger transaction cannot transfer to the same account.");
        }
    }

    private async Task EnsureObservedSolvencyAsync(
        string accountId,
        decimal requestedAmount,
        CancellationToken cancellationToken)
    {
        if (_solvencyPolicy ==
            TransferSolvencyPolicy.AllowAlways)
        {
            return;
        }

        var observedBalance = await _dbContext.LedgerEntries
            .AsNoTracking()
            .Where(entry =>
                entry.AccountId == accountId)
            .SumAsync(
                entry => entry.Amount,
                cancellationToken);

        if (observedBalance < requestedAmount)
        {
            throw new InsufficientFundsException(
                accountId,
                observedBalance,
                requestedAmount);
        }
    }

    private static void ValidateAccountsForTransfer(
        MoneyAccountEntity fromAccount,
        MoneyAccountEntity toAccount)
    {
        if (!fromAccount.IsActive)
        {
            throw new InvalidLedgerTransferException(
                $"Source account '{fromAccount.Id}' is inactive.");
        }

        if (!toAccount.IsActive)
        {
            throw new InvalidLedgerTransferException(
                $"Destination account '{toAccount.Id}' is inactive.");
        }

        if (!string.Equals(
                fromAccount.Currency,
                toAccount.Currency,
                StringComparison.Ordinal))
        {
            throw new InvalidLedgerTransferException(
                "Source and destination accounts use " +
                "different currencies.");
        }
    }
}