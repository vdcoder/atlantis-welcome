namespace Atlantis.Api.Citizens.Brain.CortexJobs
    .Completion;

public sealed record CortexTaskResultValidation
{
    public required bool IsValid { get; init; }

    public string? Message { get; init; }

    public static CortexTaskResultValidation Valid()
    {
        return new CortexTaskResultValidation
        {
            IsValid = true
        };
    }

    public static CortexTaskResultValidation Invalid(
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            message);

        return new CortexTaskResultValidation
        {
            IsValid = false,
            Message = message
        };
    }
}