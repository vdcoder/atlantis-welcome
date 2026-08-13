namespace Atlantis.Api.Persistence.Records;

public sealed class SimulateCitizenPassTaskRecord
{
    public Guid CortexTaskId { get; set; }

    public required string ExternalCitizenId { get; set; }

    public required string ExternalWorldId { get; set; }

    public required string ExternalRequestId { get; set; }

    public required string ExternalScenarioId { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }

    public CortexTaskRecord CortexTask
    {
        get;
        set;
    } = null!;

    public SimulateCitizenPassWorkOrderRecord?
        WorkOrder
    {
        get;
        set;
    }
}