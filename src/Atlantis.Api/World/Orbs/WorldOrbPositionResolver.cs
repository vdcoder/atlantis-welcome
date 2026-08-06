using Atlantis.Api.Models;
using Atlantis.Api.Models.Orbs;
using Atlantis.Api.Models.Spatial;

namespace Atlantis.Api.World.Orbs;

public sealed class WorldOrbPositionResolver
{
    public Position Resolve(
        WorldOrb orb,
        Models.World world)
    {
        ArgumentNullException.ThrowIfNull(orb);
        ArgumentNullException.ThrowIfNull(world);

        if (orb.AttachmentEntityId is null)
        {
            return orb.AttachmentPosition;
        }

        var attachmentEntity =
            world.Entities
                .SingleOrDefault(
                    entity =>
                        entity.Id ==
                        orb.AttachmentEntityId)
            ?? throw new EntityNotFoundException(
                orb.AttachmentEntityId);

        return EntityTransform.TransformLocalPosition(
            attachmentEntity,
            orb.AttachmentPosition);
    }
}