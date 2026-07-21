namespace Atlantis.Api.Economy.Ledger;

public sealed class InvalidLedgerTransferException : Exception
{
    public InvalidLedgerTransferException(string message)
        : base(message)
    {
    }
}