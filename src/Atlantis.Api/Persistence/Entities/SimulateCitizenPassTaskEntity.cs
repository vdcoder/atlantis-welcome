namespace Atlantis.Api.Persistence.Entities;

public sealed class SimulateCitizenPassTaskEntity
{
    public Guid CortexTaskId { get; set; }

    public required string ExternalCitizenId { get; set; }

    public required string ExternalWorldId { get; set; }

    public required string ExternalRequestId { get; set; }

    public required string ExternalScenarioId { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }

    public CortexTaskEntity CortexTask
    {
        get;
        set;
    } = null!;

    public SimulateCitizenPassWorkOrderEntity?
        WorkOrder
    {
        get;
        set;
    }
}