using Atlantis.Api.Common;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Mappers;
using Atlantis.Api.World;
using Atlantis.Api.World.Entities;
using CVisualAttribute = Atlantis.Api.World.VisualAttributes.VisualAttribute;
using CVoiceAttribute = Atlantis.Api.World.VoiceAttributes.VoiceAttribute;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence.Services;

public sealed class WorldStateLoader
{
    public async Task InitializeAsync(
        AtlantisDbContext dbContext,
        WorldState worldState,
        CancellationToken cancellationToken = default)
    {
        var persistedWorld =
            await dbContext.Worlds
                .Include(
                    world =>
                        world.Entities)
                    .ThenInclude(
                        entity =>
                            entity.VisualAttributes)
                        .ThenInclude(
                            visualAttribute =>
                                visualAttribute.AttributeValue)

                .Include(
                    world =>
                        world.Entities)
                    .ThenInclude(
                        entity =>
                            entity.VoiceAttributes)
                        .ThenInclude(
                            voiceAttribute =>
                                voiceAttribute.AttributeValue)

                .Include(
                    world =>
                        world.Orbs)
                    .ThenInclude(
                        orb =>
                            orb.SensoryOrb)
                        .ThenInclude(
                            sensory =>
                                sensory!.Targets)

                .Include(
                    world =>
                        world.Orbs)
                    .ThenInclude(
                        orb =>
                            orb.PositionObservationOrb)

                .FirstOrDefaultAsync(
                    world =>
                        world.WorldId ==
                            "atlantis-welcome",
                    cancellationToken);

        if (persistedWorld != null)
        {
            var world =
            new World.World
            {
                WorldId =
                    persistedWorld.WorldId,

                Time =
                    persistedWorld.Time,

                Places =
                [
                    new Place
                {
                    Id = "welcome-center",
                    Name = "Atlantis Welcome Center"
                },
                new Place
                {
                    Id = "external",
                    Name = "Outside Atlantis"
                }
                ],

                Entities = persistedWorld.Entities
                    .Select(entity => new Entity
                    {
                        Id = entity.EntityId,
                        Type = entity.Type,
                        Name = entity.Name,
                        PlaceId = entity.PlaceId,

                        Position = new Position(
                            entity.PositionX,
                            entity.PositionY,
                            entity.PositionZ),

                        Embodiment = WorldPersistenceMapper.MapEmbodiment(entity),

                        PositionChangedAt =
                            WorldPersistenceMapper.ToDateTimeOffset(
                                entity.PositionChangedAt),

                        VisualAttributes =
                            entity.VisualAttributes
                                .OrderBy(
                                    attribute =>
                                        attribute.Sequence)
                                .Select(
                                    attribute =>
                                        new CVisualAttribute(
                                            DefinitionId:
                                                attribute
                                                    .AttributeValue
                                                    .AttributeDefinitionId,

                                            ValueId:
                                                attribute
                                                    .AttributeValueId,

                                            Value:
                                                attribute
                                                    .AttributeValue
                                                    .Value,

                                            DisplayText:
                                                attribute
                                                    .AttributeValue
                                                    .DisplayText,

                                            Sequence:
                                                attribute
                                                    .Sequence))
                                .ToList(),

                        VoiceAttributes =
                            entity.VoiceAttributes
                                .OrderBy(
                                    attribute =>
                                        attribute.Sequence)
                                .Select(
                                    attribute =>
                                        new CVoiceAttribute(
                                            DefinitionId:
                                                attribute
                                                    .AttributeValue
                                                    .AttributeDefinitionId,

                                            ValueId:
                                                attribute
                                                    .AttributeValueId,

                                            Value:
                                                attribute
                                                    .AttributeValue
                                                    .Value,

                                            DisplayText:
                                                attribute
                                                    .AttributeValue
                                                    .DisplayText,

                                            Sequence:
                                                attribute
                                                    .Sequence))
                                .ToList(),
                    })
                    .ToList(),

                Orbs =
                persistedWorld.Orbs
                    .Select(WorldPersistenceMapper.MapOrb)
                    .ToList()
            };

            worldState.Initialize(world, persistedWorld.Revision);

            return;
        }

        var initialTime = DateTime.Parse(
                "2026-07-15T17:30:00Z",
                null,
                System.Globalization.DateTimeStyles.AdjustToUniversal);

        var initialWorld = new World.World
        {
            WorldId = "atlantis-welcome",
            Time = initialTime,
            Places =
            [
                new Place
            {
                Id = "welcome-center",
                Name = "Atlantis Welcome Center"
            },
            new Place
            {
                Id = "external",
                Name = "Outside Atlantis"
            }
            ],
            Entities =
            [
                new Entity
            {
                Id = "orestes",
                Type = "citizen",
                Name = "Orestes",
                PlaceId = "welcome-center",
                Position = new Position(0f, 0f, 0f),
                PositionChangedAt = initialTime,

                Embodiment = new Embodiment
                {
                    TorsoFront = Direction.UnitZ,
                    GazeDirection = Direction.UnitZ
                }
            },
            new Entity
            {
                Id = "visitor-default",
                Type = "visitor",
                Name = "Visitor",
                PlaceId = "welcome-center",
                Position = new Position(0f, 0f, -1.5f),
                PositionChangedAt = initialTime,

                Embodiment = new Embodiment
                {
                    TorsoFront = Direction.UnitZ,
                    GazeDirection = Direction.UnitZ
                }
            },
            new Entity
            {
                Id = "human:victor",
                Type = "human",
                Name = "Victor",
                PlaceId = "external",
                Position = new Position(
                    0f,
                    0f,
                    0f),
                PositionChangedAt = initialTime,
                Embodiment = null
            }
            ]
        };

        worldState.Initialize(initialWorld, 0);

        var persisted = new Records.WorldRecord
        {
            Id = Guid.NewGuid(),
            WorldId = initialWorld.WorldId,
            Time = initialWorld.Time,
            Revision = 0,
            Entities = initialWorld.Entities
                .Select(entity =>
                    new Records.EntityRecord
                    {
                        EntityId = entity.Id,
                        Type = entity.Type,
                        Name = entity.Name,
                        PlaceId = entity.PlaceId,
                        PositionX = entity.Position.X,
                        PositionY = entity.Position.Y,
                        PositionZ = entity.Position.Z,
                        PositionChangedAt =
                            entity.PositionChangedAt.UtcDateTime,
                        TorsoFrontX = entity.Embodiment?.TorsoFront.X,
                        TorsoFrontY = entity.Embodiment?.TorsoFront.Y,
                        TorsoFrontZ = entity.Embodiment?.TorsoFront.Z,
                        GazeDirectionX = entity.Embodiment?.GazeDirection.X,
                        GazeDirectionY = entity.Embodiment?.GazeDirection.Y,
                        GazeDirectionZ = entity.Embodiment?.GazeDirection.Z
                    })
                .ToList()
        };

        dbContext.Worlds.Add(persisted);
        await dbContext.SaveChangesAsync();
    }
}
