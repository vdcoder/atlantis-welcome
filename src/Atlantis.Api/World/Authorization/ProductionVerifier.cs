namespace Atlantis.Api.World.Authorization;

public sealed class ProductionVerifier
    : IProductionVerifier
{
    private readonly IWebHostEnvironment
        _environment;

    public ProductionVerifier(
        IWebHostEnvironment environment)
    {
        _environment =
            environment ??
            throw new ArgumentNullException(
                nameof(environment));
    }

    public bool IsProduction =>
        _environment.IsProduction();

    public Task VerifyBootAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsProduction)
        {
            throw new InvalidOperationException(
                "Production verification cannot run outside " +
                "the Production environment.");
        }

        // TODO:
        // Add production-only boot proof, deployment identity,
        // database identity, and world identity verification.

        return Task.CompletedTask;
    }
}