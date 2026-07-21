namespace Atlantis.Api.Economy.Ledger;

public sealed class InsufficientFundsException : Exception
{
    public InsufficientFundsException(
        string accountId,
        decimal availableBalance,
        decimal requestedAmount)
        : base(
            $"Account '{accountId}' has insufficient funds. " +
            $"Available: {availableBalance}, requested: {requestedAmount}.")
    {
        AccountId = accountId;
        AvailableBalance = availableBalance;
        RequestedAmount = requestedAmount;
    }

    public string AccountId { get; }

    public decimal AvailableBalance { get; }

    public decimal RequestedAmount { get; }
}