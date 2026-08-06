namespace Atlantis.Api.Citizens.Brain.CortexJobs.Context;

public interface IPrimedCortexTaskContextLoader
{
    Task<PrimedCortexTaskContext?> LoadAsync(
        string workerCitizenId,
        CancellationToken cancellationToken = default);
}