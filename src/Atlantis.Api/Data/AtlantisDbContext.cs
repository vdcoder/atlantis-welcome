using Atlantis.Api.Data.Entities;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Data;

public sealed class AtlantisDbContext : DbContext
{
    public AtlantisDbContext(
        DbContextOptions<AtlantisDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorldEntity> Worlds => Set<WorldEntity>();

    public DbSet<EntityEntity> Entities => Set<EntityEntity>();

    public DbSet<MoneyAccountEntity> MoneyAccounts =>
        Set<MoneyAccountEntity>();

    public DbSet<LedgerEntryEntity> LedgerEntries =>
        Set<LedgerEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureWorldEntity(modelBuilder);
        ConfigureEntityEntity(modelBuilder);

        // Finds MoneyAccountEntityConfiguration and
        // LedgerEntryEntityConfiguration automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AtlantisDbContext).Assembly);
    }

    private static void ConfigureWorldEntity(
        ModelBuilder modelBuilder)
    {
        var world = modelBuilder.Entity<WorldEntity>();

        world.ToTable("worlds");

        world.HasKey(value => value.Id);

        world.Property(value => value.Id)
            .HasColumnName("id")
            .IsRequired();

        world.Property(value => value.WorldId)
            .HasColumnName("world_id")
            .HasMaxLength(100)
            .IsRequired();

        world.Property(value => value.Time)
            .HasColumnName("time")
            .IsRequired();

        world.Property(value => value.Revision)
            .HasColumnName("revision")
            .IsRequired();

        world.HasIndex(value => value.WorldId)
            .IsUnique()
            .HasDatabaseName("ux_worlds_world_id");
    }

    private static void ConfigureEntityEntity(
        ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EntityEntity>();

        entity.ToTable("entities");

        entity.HasKey(value => value.Id);

        entity.Property(value => value.Id)
            .HasColumnName("id")
            .IsRequired();

        entity.Property(value => value.WorldId)
            .HasColumnName("world_id")
            .IsRequired();

        entity.Property(value => value.EntityId)
            .HasColumnName("entity_id")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(value => value.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(value => value.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(value => value.PlaceId)
            .HasColumnName("place_id")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(value => value.PositionX)
            .HasColumnName("position_x")
            .IsRequired();

        entity.Property(value => value.PositionY)
            .HasColumnName("position_y")
            .IsRequired();

        entity.Property(value => value.PositionZ)
            .HasColumnName("position_z")
            .IsRequired();

        entity.Property(value => value.UtteranceSequence)
            .HasColumnName("utterance_sequence");

        entity.Property(value => value.UtteranceText)
            .HasColumnName("utterance_text");

        entity.Property(value => value.UtteranceSpokenAt)
            .HasColumnName("utterance_spoken_at");

        entity.Property(value => value.PrivateMessageSequence)
            .HasColumnName("private_message_sequence");

        entity.Property(value => value.PrivateMessageSenderId)
            .HasColumnName("private_message_sender_id")
            .HasMaxLength(100);

        entity.Property(value => value.PrivateMessageText)
            .HasColumnName("private_message_text");

        entity.Property(value => value.PrivateMessageDeliveredAt)
            .HasColumnName("private_message_delivered_at");

        entity.HasOne(value => value.World)
            .WithMany(value => value.Entities)
            .HasForeignKey(value => value.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(value => new
        {
            value.WorldId,
            value.EntityId
        })
            .IsUnique()
            .HasDatabaseName("ux_entities_world_id_entity_id");
    }
}