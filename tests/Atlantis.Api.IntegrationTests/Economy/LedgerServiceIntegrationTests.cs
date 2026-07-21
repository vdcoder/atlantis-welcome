using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.Economy.Ledger;
using Atlantis.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.IntegrationTests.Economy;

public sealed class LedgerServiceIntegrationTests
{
    private const decimal TestPaymentAmount = 10.00m;

    [Fact]
    public async Task TransferAsync_CreatesBalancedEntriesAndUpdatesBalances()
    {
        await using var dbContext = CreateDbContext();

        var ledgerService = new LedgerService(dbContext);

        var fundBalanceBefore =
            await ledgerService.GetBalanceAsync(
                KnownMoneyAccounts.AtlantisDevelopmentFundId);

        var orestesBalanceBefore =
            await ledgerService.GetBalanceAsync(
                KnownMoneyAccounts.OrestesId);

        var result = await ledgerService.TransferAsync(
            KnownMoneyAccounts.AtlantisDevelopmentFundId,
            KnownMoneyAccounts.OrestesId,
            TestPaymentAmount,
            reason: "Phase 1 integration test payment",
            referenceType: "IntegrationTest",
            referenceId: "phase1-transfer-test");

        Assert.Equal(TestPaymentAmount, result.Amount);
        Assert.Equal("ATC", result.Currency);

        Assert.Equal(
            KnownMoneyAccounts.AtlantisDevelopmentFundId,
            result.DebitEntry.AccountId);

        Assert.Equal(
            -TestPaymentAmount,
            result.DebitEntry.Amount);

        Assert.Equal(
            KnownMoneyAccounts.OrestesId,
            result.CreditEntry.AccountId);

        Assert.Equal(
            TestPaymentAmount,
            result.CreditEntry.Amount);

        Assert.Equal(
            result.TransactionId,
            result.DebitEntry.TransactionId);

        Assert.Equal(
            result.TransactionId,
            result.CreditEntry.TransactionId);

        Assert.Equal(
            0m,
            result.DebitEntry.Amount +
            result.CreditEntry.Amount);

        var persistedEntries = await dbContext.LedgerEntries
            .AsNoTracking()
            .Where(entry =>
                entry.TransactionId == result.TransactionId)
            .OrderBy(entry => entry.Amount)
            .ToListAsync();

        Assert.Equal(2, persistedEntries.Count);

        Assert.Equal(
            0m,
            persistedEntries.Sum(entry => entry.Amount));

        var fundBalanceAfter =
            await ledgerService.GetBalanceAsync(
                KnownMoneyAccounts.AtlantisDevelopmentFundId);

        var orestesBalanceAfter =
            await ledgerService.GetBalanceAsync(
                KnownMoneyAccounts.OrestesId);

        Assert.Equal(
            fundBalanceBefore - TestPaymentAmount,
            fundBalanceAfter);

        Assert.Equal(
            orestesBalanceBefore + TestPaymentAmount,
            orestesBalanceAfter);
    }

    [Fact]
    public async Task TransferAsync_WhenObservedFundsAreInsufficient_ThrowsAndWritesNothing()
    {
        await using var dbContext = CreateDbContext();

        var ledgerService = new LedgerService(dbContext);

        var observedBalance =
            await ledgerService.GetBalanceAsync(
                KnownMoneyAccounts.OrestesId);

        var entryCountBefore =
            await dbContext.LedgerEntries.CountAsync();

        var requestedAmount =
            observedBalance >= 0m
                ? observedBalance + 1.00m
                : 1.00m;

        var exception =
            await Assert.ThrowsAsync<InsufficientFundsException>(
                () => ledgerService.TransferAsync(
                    KnownMoneyAccounts.OrestesId,
                    KnownMoneyAccounts.AtlantisDevelopmentFundId,
                    requestedAmount,
                    reason:
                        "Expected insufficient-funds integration test",
                    referenceType: "IntegrationTest",
                    referenceId:
                        "phase1-insufficient-funds-test"));

        Assert.Equal(
            KnownMoneyAccounts.OrestesId,
            exception.AccountId);

        Assert.Equal(
            observedBalance,
            exception.AvailableBalance);

        Assert.Equal(
            requestedAmount,
            exception.RequestedAmount);

        var entryCountAfter =
            await dbContext.LedgerEntries.CountAsync();

        Assert.Equal(
            entryCountBefore,
            entryCountAfter);
    }

    [Fact]
    public async Task EconomySeed_WhenRunRepeatedly_DoesNotDuplicateInitialIssuance()
    {
        await using var dbContext = CreateDbContext();

        await EconomySeed.EnsureSeededAsync(dbContext);
        await EconomySeed.EnsureSeededAsync(dbContext);

        var initialTransactionId =
            KnownEconomyTransactions
                .InitialDevelopmentFundAllocationId;

        var issuanceEntries = await dbContext.LedgerEntries
            .AsNoTracking()
            .Where(entry =>
                entry.TransactionId == initialTransactionId)
            .ToListAsync();

        Assert.Equal(2, issuanceEntries.Count);

        Assert.Equal(
            0m,
            issuanceEntries.Sum(entry => entry.Amount));

        Assert.Single(
            issuanceEntries,
            entry =>
                entry.AccountId ==
                KnownMoneyAccounts.AtlantisCurrencyIssuerId &&
                entry.Amount == -100_000.00m);

        Assert.Single(
            issuanceEntries,
            entry =>
                entry.AccountId ==
                KnownMoneyAccounts.AtlantisDevelopmentFundId &&
                entry.Amount == 100_000.00m);
    }

    [Fact]
    public async Task EntireLedger_IsGloballyBalanced()
    {
        await using var dbContext = CreateDbContext();

        var total = await dbContext.LedgerEntries
            .AsNoTracking()
            .SumAsync(entry => entry.Amount);

        Assert.Equal(0m, total);
    }

    [Fact]
    public async Task TransferAsync_ToSameAccount_IsRejectedAndWritesNothing()
    {
        await using var dbContext = CreateDbContext();

        var ledgerService = new LedgerService(dbContext);

        var entryCountBefore =
            await dbContext.LedgerEntries.CountAsync();

        await Assert.ThrowsAsync<InvalidLedgerTransferException>(
            () => ledgerService.TransferAsync(
                KnownMoneyAccounts.AtlantisDevelopmentFundId,
                KnownMoneyAccounts.AtlantisDevelopmentFundId,
                1.00m,
                reason:
                    "Expected same-account rejection test"));

        var entryCountAfter =
            await dbContext.LedgerEntries.CountAsync();

        Assert.Equal(
            entryCountBefore,
            entryCountAfter);
    }

    [Fact]
    public async Task TransferAsync_WithExcessPrecision_IsRejectedAndWritesNothing()
    {
        await using var dbContext = CreateDbContext();

        var ledgerService = new LedgerService(dbContext);

        var entryCountBefore =
            await dbContext.LedgerEntries.CountAsync();

        await Assert.ThrowsAsync<InvalidLedgerTransferException>(
            () => ledgerService.TransferAsync(
                KnownMoneyAccounts.AtlantisDevelopmentFundId,
                KnownMoneyAccounts.OrestesId,
                1.001m,
                reason:
                    "Expected monetary-precision rejection test"));

        var entryCountAfter =
            await dbContext.LedgerEntries.CountAsync();

        Assert.Equal(
            entryCountBefore,
            entryCountAfter);
    }

    private static AtlantisDbContext CreateDbContext()
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "ATLANTIS_TEST_CONNECTION")
            ?? "Host=localhost;Port=5432;" +
               "Database=atlantis;" +
               "Username=atlantis;" +
               "Password=atlantis-dev-password";

        var options =
            new DbContextOptionsBuilder<AtlantisDbContext>()
                .UseNpgsql(connectionString)
                .EnableDetailedErrors()
                .Options;

        return new AtlantisDbContext(options);
    }
}