using Atlantis.Api.World.EmbodiedControl;

namespace Atlantis.Api.Citizens.Control
{
    public interface ICitizenController
    {
        Task<CitizenBreath> ProduceCitizenBreathAsync(
            EmbodiedControllerContext context,
            CancellationToken cancellationToken = default);
    }
}
