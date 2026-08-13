using System.Text.Json;
using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World.Orbs;
using Atlantis.Api.World.Entities;
using Atlantis.Api.Common;

namespace Atlantis.Api.Persistence.Mappers;

public static class WorldPersistenceMapper
{
    public static Embodiment? MapEmbodiment(EntityRecord entity)
    {
        var hasNoOrientation =
            entity.TorsoFrontX is null &&
            entity.TorsoFrontY is null &&
            entity.TorsoFrontZ is null &&
            entity.GazeDirectionX is null &&
            entity.GazeDirectionY is null &&
            entity.GazeDirectionZ is null;

        if (hasNoOrientation)
        {
            return null;
        }

        var hasCompleteOrientation =
            entity.TorsoFrontX is not null &&
            entity.TorsoFrontY is not null &&
            entity.TorsoFrontZ is not null &&
            entity.GazeDirectionX is not null &&
            entity.GazeDirectionY is not null &&
            entity.GazeDirectionZ is not null;

        if (!hasCompleteOrientation)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.EntityId}' contains partial " +
                "embodiment orientation data.");
        }

        return new Embodiment
        {
            TorsoFront =
                new Direction(
                    entity.TorsoFrontX!.Value,
                    entity.TorsoFrontY!.Value,
                    entity.TorsoFrontZ!.Value)
                .Normalize(),

            GazeDirection =
                new Direction(
                    entity.GazeDirectionX!.Value,
                    entity.GazeDirectionY!.Value,
                    entity.GazeDirectionZ!.Value)
                .Normalize()
        };
    }

    public static Orb MapOrb(
        OrbRecord entity)
    {
        return entity.Type switch
        {
            "sensory" =>
                MapSensoryOrb(
                    entity),

            "position-observation" =>
                MapPositionObservationOrb(
                    entity),

            _ =>
                throw new NotSupportedException(
                    $"Unsupported persisted orb type " +
                    $"'{entity.Type}' for orb '{entity.Id}'.")
        };
    }

    public static SensoryOrb MapSensoryOrb(
        OrbRecord entity)
    {
        var sensory =
            entity.SensoryOrb ??
            throw new InvalidOperationException(
                $"Persisted sensory orb '{entity.Id}' " +
                "has no sensory-orb row.");

        var modality =
            (SensoryModality)
                sensory.Modality;

        var content =
            modality switch
            {
                SensoryModality.Auditory or
                SensoryModality.Tactile =>
                    MapTextContent(
                        entity.Id,
                        sensory),

                _ =>
                    throw new NotSupportedException(
                        $"Unsupported persisted sensory modality " +
                        $"'{sensory.Modality}' for orb '{entity.Id}'.")
            };

        return new SensoryOrb(
            id:
                entity.Id,

            createdAt:
                ToDateTimeOffset(
                    entity.CreatedAt),

            expiresAt:
                entity.ExpiresAt is null
                    ? null
                    : ToDateTimeOffset(
                        entity.ExpiresAt.Value),

            sourceEntityId:
                entity.SourceEntityId,

            attachmentEntityId:
                entity.AttachmentEntityId,

            attachmentPosition:
                new Position(
                    entity.AttachmentPositionX,
                    entity.AttachmentPositionY,
                    entity.AttachmentPositionZ),

            radius:
                entity.Radius,

            modality:
                modality,

            content:
                content,

            intensity:
                sensory.Intensity,

            targetEntityIds:
                sensory.Targets.Count == 0
                    ? null
                    : sensory.Targets
                        .Select(
                            target =>
                                target.TargetEntityId)
                        .ToHashSet());
    }

    public static string MapTextContent(
        Guid orbId,
        SensoryOrbRecord sensory)
    {
        var payload =
            JsonSerializer.Deserialize<TextSensoryOrbPayload>(
                sensory.Payload);

        if (payload is null ||
            string.IsNullOrWhiteSpace(
                payload.Text))
        {
            throw new InvalidOperationException(
                $"Persisted sensory orb '{orbId}' " +
                "contains an invalid text payload.");
        }

        return payload.Text;
    }

    public static PositionObservationOrb
        MapPositionObservationOrb(
            OrbRecord entity)
    {
        var positionObservation =
            entity.PositionObservationOrb ??
            throw new InvalidOperationException(
                $"Persisted position observation orb " +
                $"'{entity.Id}' has no subtype row.");

        return new PositionObservationOrb(
            id:
                entity.Id,

            createdAt:
                ToDateTimeOffset(
                    entity.CreatedAt),

            expiresAt:
                entity.ExpiresAt is null
                    ? null
                    : ToDateTimeOffset(
                        entity.ExpiresAt.Value),

            sourceEntityId:
                entity.SourceEntityId
                ?? throw new InvalidOperationException(
                    $"Position observation orb " +
                    $"'{entity.Id}' has no source entity."),

            observedEntityId:
                positionObservation.ObservedEntityId,

            observedPosition:
                new Position(
                    entity.AttachmentPositionX,
                    entity.AttachmentPositionY,
                    entity.AttachmentPositionZ),

            positionFirstRecordedAt:
                ToDateTimeOffset(
                    positionObservation.PositionFirstRecordedAt));
    }

    public static DateTimeOffset ToDateTimeOffset(
        DateTime value)
    {
        var utc =
            value.Kind == DateTimeKind.Utc
                ? value
                : DateTime.SpecifyKind(
                    value,
                    DateTimeKind.Utc);

        return new DateTimeOffset(
            utc);
    }
}