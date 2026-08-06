using Atlantis.Api.Citizens.Brain.CortexTools;

namespace Atlantis.Api.Citizens.Runtime;

public sealed class CortexToolCallExecutor
{
    private readonly CompleteCortexTask
        _completeCortexTask;

    private readonly ILogger<CortexToolCallExecutor>
        _logger;

    public CortexToolCallExecutor(
        CompleteCortexTask completeCortexTask,
        ILogger<CortexToolCallExecutor> logger)
    {
        _completeCortexTask =
            completeCortexTask;

        _logger =
            logger;
    }

    public async Task ExecuteAsync(
        string workerCitizenId,
        CortexToolCall call,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        ArgumentNullException.ThrowIfNull(
            call);

        switch (call)
        {
            case CompleteCortexTaskCall complete:
                {
                    var result =
                        await _completeCortexTask
                            .CompleteAsync(
                                workerCitizenId,
                                complete.AssignmentId,
                                complete.ResultSerialized,
                                cancellationToken);

                    _logger.LogInformation(
                        "\x1b[35mCitizen\x1b[0m {CitizenId} Cortex completion for " +
                        "assignment {AssignmentId} produced outcome " +
                        "{Outcome}.",
                        workerCitizenId,
                        complete.AssignmentId,
                        result.Outcome);

                    return;
                }

            default:
                throw new NotSupportedException(
                    $"Unsupported Cortex tool call " +
                    $"'{call.GetType().Name}'.");
        }
    }
}