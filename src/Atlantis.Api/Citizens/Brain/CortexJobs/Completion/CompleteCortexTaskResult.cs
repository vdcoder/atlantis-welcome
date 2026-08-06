namespace Atlantis.Api.Citizens.Brain.CortexJobs.Completion
{
    public sealed record CompleteCortexTaskResult
    {
        public required CompleteCortexTaskOutcome Outcome { get; init; }

        public Guid? CortexTaskResultId { get; init; }

        public string? ValidationMessage { get; init; }

        public bool WasCompleted =>
            Outcome == CompleteCortexTaskOutcome.Completed;
    }
}
