namespace Atlantis.Api.Citizens.Brain.CortexJobs.Context;

public sealed record PrimedCortexTaskContext
{
    public PrimedCortexTaskContext(
        Guid assignmentId,
        Guid cortexTaskId,
        string cortexTaskType,
        string instructionsAndInputs)
    {
        if (assignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assignment ID cannot be empty.",
                nameof(assignmentId));
        }

        if (cortexTaskId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cortex task ID cannot be empty.",
                nameof(cortexTaskId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            cortexTaskType);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            instructionsAndInputs);

        AssignmentId = assignmentId;
        CortexTaskId = cortexTaskId;
        CortexTaskType = cortexTaskType;
        InstructionsAndInputs = instructionsAndInputs;
    }

    public Guid AssignmentId { get; }

    public Guid CortexTaskId { get; }

    public string CortexTaskType { get; }

    public string InstructionsAndInputs { get; }
}