using Atlantis.Api.Citizens.Brain.CortexJobs.Completion;

namespace Atlantis.Api.Citizens.Brain.CortexTools;

public sealed class CompleteCortexTask
{
    private readonly CortexTaskCompletionService
        _completionService;

    public CompleteCortexTask(
        CortexTaskCompletionService completionService)
    {
        _completionService = completionService;
    }

    public Task<CompleteCortexTaskResult>
        CompleteAsync(
            string workerCitizenId,
            Guid assignmentId,
            string resultSerialized,
            CancellationToken cancellationToken = default)
    {
        return _completionService.CompleteAsync(
            new CompleteCortexTaskRequest
            {
                WorkerCitizenId =
                    workerCitizenId,

                AssignmentId =
                    assignmentId,

                ResultSerialized =
                    resultSerialized
            },
            cancellationToken);
    }
}