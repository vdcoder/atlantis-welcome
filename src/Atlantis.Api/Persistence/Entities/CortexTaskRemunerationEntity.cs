namespace Atlantis.Api.Persistence.Entities;

public sealed class CortexTaskRemunerationEntity
{
    public Guid Id
    {
        get;
        set;
    }

    public Guid CortexTaskId
    {
        get;
        set;
    }

    public Guid CortexTaskAssignmentId
    {
        get;
        set;
    }

    public required string WorkerCitizenId
    {
        get;
        set;
    }

    public required string EmployerAccountId
    {
        get;
        set;
    }

    public decimal Amount
    {
        get;
        set;
    }

    public required string Currency
    {
        get;
        set;
    }

    public int Status
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }

    public DateTimeOffset? PaidAt
    {
        get;
        set;
    }

    public Guid? LedgerTransactionId
    {
        get;
        set;
    }
}