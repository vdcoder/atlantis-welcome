namespace Atlantis.Api.Citizens.Brain.CortexJobs.Assignments;

public sealed record CortexTaskPrimeResult
{
    public required CortexTaskPrimeOutcome Outcome
    {
        get;
        init;
    }

    public Guid? CortexTaskId
    {
        get;
        init;
    }

    public Guid? CortexTaskAssignmentId
    {
        get;
        init;
    }

    public bool WasPrimed =>
        Outcome == CortexTaskPrimeOutcome.Primed;

    public static CortexTaskPrimeResult Primed(
        Guid cortexTaskId,
        Guid cortexTaskAssignmentId)
    {
        if (cortexTaskId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cortex task ID cannot be empty.",
                nameof(cortexTaskId));
        }

        if (cortexTaskAssignmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cortex task assignment ID cannot be empty.",
                nameof(cortexTaskAssignmentId));
        }

        return new CortexTaskPrimeResult
        {
            Outcome =
                CortexTaskPrimeOutcome.Primed,

            CortexTaskId =
                cortexTaskId,

            CortexTaskAssignmentId =
                cortexTaskAssignmentId
        };
    }

    public static CortexTaskPrimeResult NotPrimed(
        CortexTaskPrimeOutcome outcome)
    {
        if (!Enum.IsDefined(outcome))
        {
            throw new ArgumentOutOfRangeException(
                nameof(outcome),
                outcome,
                "Unknown Cortex task prime outcome.");
        }

        if (outcome ==
            CortexTaskPrimeOutcome.Primed)
        {
            throw new ArgumentException(
                "Use Primed() to create a successful result.",
                nameof(outcome));
        }

        return new CortexTaskPrimeResult
        {
            Outcome = outcome
        };
    }
}