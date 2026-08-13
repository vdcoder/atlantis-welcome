using Atlantis.Api.Common;
using Atlantis.Api.World.Spatial;

namespace Atlantis.Api.World.Orbs;

public sealed class OrbPositionResolver
{
    public Position Resolve(
        Orb orb,
        World world)
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

        var entityTorsoFront =
            attachmentEntity.Embodiment?.TorsoFront
            ?? throw new EmbodimentNotFoundException(
                orb.AttachmentEntityId);

        return TransformPosition.TransformLocalPosition(
            attachmentEntity.Position,
            entityTorsoFront,
            orb.AttachmentPosition);
    }
}