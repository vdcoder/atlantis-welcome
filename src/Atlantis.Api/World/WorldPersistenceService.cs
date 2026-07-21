using Atlantis.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.World
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

                        if (entity.CurrentUtterance != null)
                        {
                            persistedEntity.UtteranceText = entity.CurrentUtterance.Text;
                            persistedEntity.UtteranceSequence = entity.CurrentUtterance.Sequence;
                            persistedEntity.UtteranceSpokenAt = entity.CurrentUtterance.SpokenAt.UtcDateTime;
                        }
                        else
                        {
                            persistedEntity.UtteranceText = null;
                            persistedEntity.UtteranceSequence = null;
                            persistedEntity.UtteranceSpokenAt = null;
                        }

                        // Update private message fields
                        if (entity.CurrentPrivateMessage is not null)
                        {
                            persistedEntity.PrivateMessageSequence =
                                entity.CurrentPrivateMessage.Sequence;

                            persistedEntity.PrivateMessageSenderId =
                                entity.CurrentPrivateMessage.SenderId;

                            persistedEntity.PrivateMessageText =
                                entity.CurrentPrivateMessage.Text;

                            persistedEntity.PrivateMessageDeliveredAt =
                                entity.CurrentPrivateMessage
                                    .DeliveredAt
                                    .UtcDateTime;
                        }
                        else
                        {
                            persistedEntity.PrivateMessageSequence = null;
                            persistedEntity.PrivateMessageSenderId = null;
                            persistedEntity.PrivateMessageText = null;
                            persistedEntity.PrivateMessageDeliveredAt = null;
                        }
                    }
                }

                dbContext.Worlds.Update(persistedWorld);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
