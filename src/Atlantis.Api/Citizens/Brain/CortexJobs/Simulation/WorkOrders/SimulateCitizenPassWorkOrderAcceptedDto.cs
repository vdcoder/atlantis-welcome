namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Simulation.WorkOrders;

public sealed record
    SimulateCitizenPassWorkOrderAcceptedDto
{
    public required Guid WorkOrderId
    {
        get;
        init;
    }

    public required Guid CortexTaskId
    {
        get;
        init;
    }

    public required SimulateCitizenPassWorkOrderStatus
        Status
    {
        get;
        init;
    }

    public required bool Created
    {
        get;
        init;
    }
}