using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.Economy.Currency;
using Atlantis.Api.Economy.Ledger;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public static class EconomySeed
{
    private const decimal InitialDevelopmentFundAllocation =
        100_000.00m;

    private const string CurrencyIssuanceReferenceType =
        "CurrencyIssuance";

    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureAccountsExistAsync(
            dbContext,
            cancellationToken);

        // Persist the accounts before creating ledger rows that reference them.
        await dbContext.SaveChangesAsync(cancellationToken);

        await EnsureInitialDevelopmentFundAllocationAsync(
            dbContext,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureAccountsExistAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        await EnsureAccountExistsAsync(
            dbContext,
            new MoneyAccountEntity
            {
                Id = KnownMoneyAccounts.AtlantisCurrencyIssuerId,
                OwnerId = "atlantis-currency",
                OwnerType = (int)AccountOwnerType.System,
                AccountType = (int)AccountType.CurrencyIssuer,
                Currency = AtlantisCurrency.Code,
                Name = "Atlantis Currency Issuer",
                CreatedAt = now,
                IsActive = true
            },
            cancellationToken);

        await EnsureAccountExistsAsync(
            dbContext,
            new MoneyAccountEntity
            {
                Id = KnownMoneyAccounts.AtlantisDevelopmentFundId,
                OwnerId = "atlantis-development",
                OwnerType = (int)AccountOwnerType.Institution,
                AccountType = (int)AccountType.DevelopmentFund,
                Currency = AtlantisCurrency.Code,
                Name = "Atlantis Development Fund",
                CreatedAt = now,
                IsActive = true
            },
            cancellationToken);

        await EnsureAccountExistsAsync(
            dbContext,
            new MoneyAccountEntity
            {
                Id = KnownMoneyAccounts.OrestesId,
                OwnerId = "orestes",
                OwnerType = (int)AccountOwnerType.Citizen,
                AccountType = (int)AccountType.CitizenWallet,
                Currency = AtlantisCurrency.Code,
                Name = "Orestes",
                CreatedAt = now,
                IsActive = true
            },
            cancellationToken);
    }

    private static async Task EnsureInitialDevelopmentFundAllocationAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var transactionId =
            KnownEconomyTransactions.InitialDevelopmentFundAllocationId;

        var alreadyExists = await dbContext.LedgerEntries
            .AnyAsync(
                entry => entry.TransactionId == transactionId,
                cancellationToken);

        if (alreadyExists)
        {
            return;
        }

        var createdAt = DateTimeOffset.UtcNow;
        var amount = AtlantisCurrency.Normalize(
            InitialDevelopmentFundAllocation);

        var issuerEntry = new LedgerEntryEntity
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            AccountId =
                KnownMoneyAccounts.AtlantisCurrencyIssuerId,
            Amount = -amount,
            Currency = AtlantisCurrency.Code,
            Reason =
                "Initial Atlantis Development Fund allocation",
            ReferenceType = CurrencyIssuanceReferenceType,
            ReferenceId =
                KnownEconomyTransactions
                    .InitialDevelopmentFundAllocationReferenceId,
            CreatedAt = createdAt
        };

        var developmentFundEntry = new LedgerEntryEntity
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            AccountId =
                KnownMoneyAccounts.AtlantisDevelopmentFundId,
            Amount = amount,
            Currency = AtlantisCurrency.Code,
            Reason =
                "Initial Atlantis Development Fund allocation",
            ReferenceType = CurrencyIssuanceReferenceType,
            ReferenceId =
                KnownEconomyTransactions
                    .InitialDevelopmentFundAllocationReferenceId,
            CreatedAt = createdAt
        };

        ValidateBalancedTransaction(
            issuerEntry,
            developmentFundEntry);

        dbContext.LedgerEntries.AddRange(
            issuerEntry,
            developmentFundEntry);
    }

    private static async Task EnsureAccountExistsAsync(
        AtlantisDbContext dbContext,
        MoneyAccountEntity account,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.MoneyAccounts
            .AnyAsync(
                existing => existing.Id == account.Id,
                cancellationToken);

        if (!exists)
        {
            dbContext.MoneyAccounts.Add(account);
        }
    }

    private static void ValidateBalancedTransaction(
        params LedgerEntryEntity[] entries)
    {
        var total = entries.Sum(entry => entry.Amount);

        if (total != 0m)
        {
            throw new InvalidOperationException(
                $"Ledger transaction is not balanced. Total: {total}.");
        }

        var currencies = entries
            .Select(entry => entry.Currency)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (currencies.Length != 1)
        {
            throw new InvalidOperationException(
                "Ledger transaction contains multiple currencies.");
        }
    }
}