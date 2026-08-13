using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World.Orbs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Atlantis.Api.World;

namespace Atlantis.Api.Persistence.Services
{
    public sealed class WorldPersistenceService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public WorldPersistenceService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task SaveAsync(
            WorldState worldState,
            CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<AtlantisDbContext>();

            // Update persisted world state and entities
            var persistedWorld = await dbContext.Worlds
                .Include(world => world.Entities)
                .Include(world => world.Orbs)
                    .ThenInclude(orb => orb.SensoryOrb)
                        .ThenInclude(sensory => sensory!.Targets)
                .Include(world => world.Orbs)
                    .ThenInclude(orb => orb.PositionObservationOrb)
                .FirstOrDefaultAsync(
                    world =>
                        world.WorldId ==
                        worldState.World.WorldId,
                    cancellationToken);

            if (persistedWorld != null)
            {
                persistedWorld.Time = worldState.World.Time;
                persistedWorld.Revision = worldState.Revision;

                // Update entities
                foreach (var entity in worldState.World.Entities)
                {
                    var persistedEntity = persistedWorld.Entities
                        .FirstOrDefault(e => e.EntityId == entity.Id);

                    if (persistedEntity != null)
                    {
                        persistedEntity.PositionX = entity.Position.X;
                        persistedEntity.PositionY = entity.Position.Y;
                        persistedEntity.PositionZ = entity.Position.Z;

                        persistedEntity.PositionChangedAt =
                            entity.PositionChangedAt.UtcDateTime;

                        // Embodiment
                        if (entity.Embodiment is not null)
                        {
                            persistedEntity.TorsoFrontX =
                                entity.Embodiment.TorsoFront.X;

                            persistedEntity.TorsoFrontY =
                                entity.Embodiment.TorsoFront.Y;

                            persistedEntity.TorsoFrontZ =
                                entity.Embodiment.TorsoFront.Z;

                            persistedEntity.GazeDirectionX =
                                entity.Embodiment.GazeDirection.X;

                            persistedEntity.GazeDirectionY =
                                entity.Embodiment.GazeDirection.Y;

                            persistedEntity.GazeDirectionZ =
                                entity.Embodiment.GazeDirection.Z;
                        }
                        else
                        {
                            persistedEntity.TorsoFrontX = null;
                            persistedEntity.TorsoFrontY = null;
                            persistedEntity.TorsoFrontZ = null;
                            persistedEntity.GazeDirectionX = null;
                            persistedEntity.GazeDirectionY = null;
                            persistedEntity.GazeDirectionZ = null;
                        }
                    }
                }

                // Update orbs
                foreach (var orb in worldState.World.Orbs)
                {
                    var persistedOrb =
                        persistedWorld.Orbs
                            .SingleOrDefault(
                                value =>
                                    value.Id == orb.Id);

                    if (persistedOrb is not null)
                    {
                        continue;
                    }

                    switch (orb)
                    {
                        case SensoryOrb sensoryOrb:
                            {
                                var newPersistedOrb =
                                    MapSensoryOrb(
                                        sensoryOrb,
                                        persistedWorld.Id);

                                newPersistedOrb.World =
                                    persistedWorld;

                                dbContext.Orbs.Add(
                                    newPersistedOrb);

                                break;
                            }

                        case PositionObservationOrb positionObservationOrb:
                            {
                                var newPersistedOrb =
                                    MapPositionObservationOrb(
                                        positionObservationOrb,
                                        persistedWorld.Id);

                                newPersistedOrb.World =
                                    persistedWorld;

                                dbContext.Orbs.Add(
                                    newPersistedOrb);

                                break;
                            }

                        default:
                            throw new NotSupportedException(
                                $"Unsupported orb type: " +
                                $"{orb.GetType().Name}");
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private static OrbRecord MapSensoryOrb(
            SensoryOrb orb,
            Guid worldId)
        {
            var payload =
                orb.Modality switch
                {
                    SensoryModality.Auditory or
                    SensoryModality.Tactile =>
                        JsonSerializer.SerializeToDocument(
                            new TextSensoryOrbPayload
                            {
                                Text =
                                    orb.Content
                            }),

                    _ =>
                        throw new NotSupportedException(
                            $"Unsupported sensory modality: " +
                            $"{orb.Modality}")
                };

            var persistedOrb =
                new OrbRecord
                {
                    Id =
                        orb.Id,

                    WorldId =
                        worldId,

                    Type =
                        "sensory",

                    CreatedAt =
                        orb.CreatedAt.UtcDateTime,

                    ExpiresAt =
                        orb.ExpiresAt?.UtcDateTime,

                    SourceEntityId =
                        orb.SourceEntityId,

                    AttachmentEntityId =
                        orb.AttachmentEntityId,

                    AttachmentPositionX =
                        orb.AttachmentPosition.X,

                    AttachmentPositionY =
                        orb.AttachmentPosition.Y,

                    AttachmentPositionZ =
                        orb.AttachmentPosition.Z,

                    Radius =
                        orb.Radius
                };

            var persistedSensoryOrb =
                new SensoryOrbRecord
                {
                    OrbId =
                        persistedOrb.Id,

                    Modality =
                        (int)orb.Modality,

                    Intensity =
                        orb.Intensity,

                    Payload =
                        payload,

                    Orb =
                        persistedOrb,

                    Targets =
                        orb.TargetEntityIds is null
                            ? []
                            : orb.TargetEntityIds
                                .Select(
                                    targetEntityId =>
                                        new SensoryOrbTargetRecord
                                        {
                                            OrbId =
                                                persistedOrb.Id,

                                            TargetEntityId =
                                                targetEntityId
                                        })
                                .ToList()
                };

            persistedOrb.SensoryOrb =
                persistedSensoryOrb;

            return persistedOrb;
        }

        private static OrbRecord MapPositionObservationOrb(
            PositionObservationOrb orb,
            Guid worldId)
        {
            var persistedOrb =
                new OrbRecord
                {
                    Id =
                        orb.Id,

                    WorldId =
                        worldId,

                    Type =
                        "position-observation",

                    CreatedAt =
                        orb.CreatedAt.UtcDateTime,

                    ExpiresAt =
                        orb.ExpiresAt?.UtcDateTime,

                    SourceEntityId =
                        orb.SourceEntityId,

                    AttachmentEntityId =
                        null,

                    AttachmentPositionX =
                        orb.ObservedPosition.X,

                    AttachmentPositionY =
                        orb.ObservedPosition.Y,

                    AttachmentPositionZ =
                        orb.ObservedPosition.Z,

                    Radius =
                        0f
                };

            persistedOrb.PositionObservationOrb =
                new PositionObservationOrbRecord
                {
                    OrbId =
                        persistedOrb.Id,

                    ObservedEntityId =
                        orb.ObservedEntityId,

                    PositionFirstRecordedAt =
                        orb.PositionFirstRecordedAt.UtcDateTime,

                    Orb =
                        persistedOrb
                };

            return persistedOrb;
        }
    }
}
