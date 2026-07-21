using Atlantis.Api.Data;
using Atlantis.Api.Economy.Contracts;
using Atlantis.Api.Economy.Ledger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Controllers;

[ApiController]
[Route("api/economy")]
public sealed class EconomyController : ControllerBase
{
    private readonly AtlantisDbContext _dbContext;
    private readonly LedgerService _ledgerService;

    public EconomyController(
        AtlantisDbContext dbContext,
        LedgerService ledgerService)
    {
        _dbContext = dbContext;
        _ledgerService = ledgerService;
    }

    [HttpGet("accounts")]
    public async Task<ActionResult<IReadOnlyList<AccountBalanceResponse>>>
        GetAccountsAsync(
            CancellationToken cancellationToken)
    {
        var accounts = await _dbContext.MoneyAccounts
            .AsNoTracking()
            .OrderBy(account => account.Name)
            .ToListAsync(cancellationToken);

        var balances = await _dbContext.LedgerEntries
            .AsNoTracking()
            .GroupBy(entry => entry.AccountId)
            .Select(group => new
            {
                AccountId = group.Key,
                Balance = group.Sum(entry => entry.Amount)
            })
            .ToDictionaryAsync(
                item => item.AccountId,
                item => item.Balance,
                cancellationToken);

        var response = accounts
            .Select(account => new AccountBalanceResponse
            {
                AccountId = account.Id,
                AccountName = account.Name,
                Balance = balances.GetValueOrDefault(
                    account.Id,
                    0m),
                Currency = account.Currency,
                IsActive = account.IsActive
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("accounts/{accountId}/balance")]
    public async Task<ActionResult<AccountBalanceResponse>>
        GetBalanceAsync(
            string accountId,
            CancellationToken cancellationToken)
    {
        var account = await _dbContext.MoneyAccounts
            .AsNoTracking()
            .SingleOrDefaultAsync(
                value => value.Id == accountId,
                cancellationToken);

        if (account is null)
        {
            return NotFound(new
            {
                message =
                    $"Money account '{accountId}' was not found."
            });
        }

        var balance = await _ledgerService.GetBalanceAsync(
            accountId,
            cancellationToken);

        return Ok(new AccountBalanceResponse
        {
            AccountId = account.Id,
            AccountName = account.Name,
            Balance = balance,
            Currency = account.Currency,
            IsActive = account.IsActive
        });
    }

    [HttpGet("accounts/{accountId}/ledger")]
    public async Task<ActionResult<IReadOnlyList<LedgerEntryResponse>>>
        GetLedgerAsync(
            string accountId,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var entries = await _ledgerService
                .GetRecentEntriesAsync(
                    accountId,
                    limit,
                    cancellationToken);

            var response = entries
                .Select(entry => new LedgerEntryResponse
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
                .ToList();

            return Ok(response);
        }
        catch (MoneyAccountNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}