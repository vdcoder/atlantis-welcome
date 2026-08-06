namespace Atlantis.Api.World.Authorization;

public interface IProductionVerifier
{
    bool IsProduction { get; }

    Task VerifyBootAsync(
        CancellationToken cancellationToken = default);
}