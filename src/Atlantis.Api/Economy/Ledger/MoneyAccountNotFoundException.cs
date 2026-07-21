namespace Atlantis.Api.Economy.Ledger;

public sealed class MoneyAccountNotFoundException : Exception
{
    public MoneyAccountNotFoundException(string accountId)
        : base($"Money account '{accountId}' was not found.")
    {
        AccountId = accountId;
    }

    public string AccountId { get; }
}