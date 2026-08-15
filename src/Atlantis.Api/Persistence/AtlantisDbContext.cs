using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public sealed class AtlantisDbContext : DbContext
{
    public AtlantisDbContext(
        DbContextOptions<AtlantisDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorldRecord> Worlds => Set<WorldRecord>();

    public DbSet<EntityRecord> Entities => Set<EntityRecord>();

    public DbSet<VisualAttributeDefinitionRecord>
    VisualAttributeDefinitions =>
        Set<VisualAttributeDefinitionRecord>();

    public DbSet<VisualAttributeValueRecord>
        VisualAttributeValues =>
            Set<VisualAttributeValueRecord>();

    public DbSet<EntityVisualAttributeRecord>
        EntityVisualAttributes =>
            Set<EntityVisualAttributeRecord>();

    public DbSet<VoiceAttributeDefinitionRecord>
    VoiceAttributeDefinitions =>
        Set<VoiceAttributeDefinitionRecord>();

    public DbSet<VoiceAttributeValueRecord>
        VoiceAttributeValues =>
            Set<VoiceAttributeValueRecord>();

    public DbSet<EntityVoiceAttributeRecord>
        EntityVoiceAttributes =>
            Set<EntityVoiceAttributeRecord>();

    public DbSet<OrbRecord> Orbs =>
        Set<OrbRecord>();

    public DbSet<SensoryOrbRecord> SensoryOrbs =>
        Set<SensoryOrbRecord>();

    public DbSet<MoneyAccountRecord> MoneyAccounts =>
        Set<MoneyAccountRecord>();

    public DbSet<LedgerEntryRecord> LedgerEntries =>
        Set<LedgerEntryRecord>();

    public DbSet<CortexJobDefinitionRecord> CortexJobDefinitions =>
        Set<CortexJobDefinitionRecord>();

    public DbSet<CortexJobScheduleConditionRecord> CortexJobScheduleConditions =>
        Set<CortexJobScheduleConditionRecord>();

    public DbSet<CortexJobApplicationRecord> CortexJobApplications =>
        Set<CortexJobApplicationRecord>();

    public DbSet<WorkerCortexJobQualificationRecord> WorkerCortexJobQualifications =>
            Set<WorkerCortexJobQualificationRecord>();

    public DbSet<WorkerCortexJobAvailabilityRecord> WorkerCortexJobAvailability =>
            Set<WorkerCortexJobAvailabilityRecord>();

    public DbSet<CortexTaskRecord> CortexTasks =>
        Set<CortexTaskRecord>();

    public DbSet<SimulateCitizenPassTaskRecord> SimulateCitizenPassTasks =>
        Set<SimulateCitizenPassTaskRecord>();

    public DbSet<CortexTaskRemunerationRecord> CortexTaskRemunerations =>
        Set<CortexTaskRemunerationRecord>();

    public DbSet<SimulateCitizenPassWorkOrderRecord> SimulateCitizenPassWorkOrders =>
        Set<SimulateCitizenPassWorkOrderRecord>();

    public DbSet<CortexTaskAssignmentRecord> CortexTaskAssignments =>
        Set<CortexTaskAssignmentRecord>();

    public DbSet<CortexTaskResultRecord> CortexTaskResults =>
        Set<CortexTaskResultRecord>();

    public DbSet<CortexJobInboxMessageRecord> CortexJobInboxMessages =>
        Set<CortexJobInboxMessageRecord>();

    public DbSet<CitizenSponsorshipRecord> CitizenSponsorships =>
        Set<CitizenSponsorshipRecord>();

    public DbSet<DevelopmentPredictionRequestRecord>
        DevelopmentPredictionRequests =>
            Set<DevelopmentPredictionRequestRecord>();

    public DbSet<DevelopmentRecordedPredictionRecord>
        DevelopmentRecordedPredictions =>
            Set<DevelopmentRecordedPredictionRecord>();

    public DbSet<PositionObservationOrbRecord>
        PositionObservationOrbs =>
            Set<PositionObservationOrbRecord>();

    public DbSet<SensoryOrbTargetRecord> SensoryOrbTargets =>
        Set<SensoryOrbTargetRecord>();

    public DbSet<CitizenMemoryStreamEntryRecord> CitizenMemoryStreamEntries =>
        Set<CitizenMemoryStreamEntryRecord>();

    public DbSet<CitizenWorkingMemoryLineRecord> CitizenWorkingMemoryLines =>
        Set<CitizenWorkingMemoryLineRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureWorldEntity(modelBuilder);
        ConfigureEntityEntity(modelBuilder);
        ConfigureOrbEntity(modelBuilder);
        ConfigureSensoryOrbEntity(modelBuilder);
        ConfigurePositionObservationOrbEntity(modelBuilder);
        ConfigureSensoryOrbTargetEntity(modelBuilder);

        // Finds MoneyAccountEntityConfiguration and
        // LedgerEntryEntityConfiguration automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AtlantisDbContext).Assembly);
    }

    private static void ConfigureWorldEntity(
        ModelBuilder modelBuilder)
    {
        var world = modelBuilder.Entity<WorldRecord>();

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
        var entity = modelBuilder.Entity<EntityRecord>();

        entity.ToTable("entities");

        entity.HasKey(
            value =>
                value.EntityId);

        entity.Property(
                value =>
                    value.EntityId)
            .HasColumnName(
                "id")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(value => value.WorldId)
            .HasColumnName("world_id")
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

        entity.Property(value => value.PositionChangedAt)
            .HasColumnName("position_changed_at")
            .IsRequired();

        entity.Property(value => value.TorsoFrontX)
            .HasColumnName("torso_front_x");

        entity.Property(value => value.TorsoFrontY)
            .HasColumnName("torso_front_y");

        entity.Property(value => value.TorsoFrontZ)
            .HasColumnName("torso_front_z");

        entity.Property(value => value.GazeDirectionX)
            .HasColumnName("gaze_direction_x");

        entity.Property(value => value.GazeDirectionY)
            .HasColumnName("gaze_direction_y");

        entity.Property(value => value.GazeDirectionZ)
            .HasColumnName("gaze_direction_z");

        entity.HasOne(value => value.World)
            .WithMany(value => value.Entities)
            .HasForeignKey(value => value.WorldId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureOrbEntity(
        ModelBuilder modelBuilder)
    {
        var orb =
            modelBuilder.Entity<OrbRecord>();

        orb.ToTable(
            "orbs",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_orbs_radius",
                    "radius >= 0");

                table.HasCheckConstraint(
                    "ck_orbs_expiration",
                    "expires_at IS NULL OR expires_at > created_at");
            });

        orb.HasKey(
            value => value.Id);

        orb.Property(
                value => value.Id)
            .HasColumnName("id")
            .IsRequired();

        orb.Property(
                value => value.WorldId)
            .HasColumnName("world_id")
            .IsRequired();

        orb.Property(
                value => value.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired();

        orb.Property(
                value => value.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        orb.Property(
                value => value.ExpiresAt)
            .HasColumnName("expires_at");

        orb.Property(
                value => value.SourceEntityId)
            .HasColumnName("source_entity_id")
            .HasMaxLength(100);

        orb.Property(
                value => value.AttachmentEntityId)
            .HasColumnName("attachment_entity_id")
            .HasMaxLength(100);

        orb.Property(
                value => value.AttachmentPositionX)
            .HasColumnName("attachment_position_x")
            .IsRequired();

        orb.Property(
                value => value.AttachmentPositionY)
            .HasColumnName("attachment_position_y")
            .IsRequired();

        orb.Property(
                value => value.AttachmentPositionZ)
            .HasColumnName("attachment_position_z")
            .IsRequired();

        orb.Property(
                value => value.Radius)
            .HasColumnName("radius")
            .IsRequired();

        orb.HasOne(
                value => value.World)
            .WithMany(
                value => value.Orbs)
            .HasForeignKey(
                value => value.WorldId)
            .OnDelete(
                DeleteBehavior.Cascade);

        orb.HasIndex(
                value => value.WorldId)
            .HasDatabaseName(
                "ix_orbs_world_id");

        orb.HasIndex(
                value => new
                {
                    value.WorldId,
                    value.ExpiresAt
                })
            .HasDatabaseName(
                "ix_orbs_world_id_expires_at");
    }

    private static void ConfigureSensoryOrbEntity(
        ModelBuilder modelBuilder)
    {
        var sensory =
            modelBuilder.Entity<SensoryOrbRecord>();

        sensory.ToTable(
            "sensory_orbs",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_sensory_orbs_intensity",
                    "intensity >= 0 AND intensity <= 1");
            });

        sensory.HasKey(
            value => value.OrbId);

        sensory.Property(
                value => value.OrbId)
            .HasColumnName("orb_id")
            .IsRequired();

        sensory.Property(
                value => value.Modality)
            .HasColumnName("modality")
            .IsRequired();

        sensory.Property(
                value => value.Intensity)
            .HasColumnName("intensity")
            .IsRequired();

        sensory.Property(
                value => value.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .IsRequired();

        sensory.HasOne(
                value => value.Orb)
            .WithOne(
                value => value.SensoryOrb)
            .HasForeignKey<SensoryOrbRecord>(
                value => value.OrbId)
            .OnDelete(
                DeleteBehavior.Cascade);
    }

    private static void ConfigurePositionObservationOrbEntity(
        ModelBuilder modelBuilder)
    {
        var positionObservation =
            modelBuilder.Entity<PositionObservationOrbRecord>();

        positionObservation.ToTable(
            "position_observation_orbs");

        positionObservation.HasKey(
            value => value.OrbId);

        positionObservation.Property(
                value => value.OrbId)
            .HasColumnName("orb_id")
            .IsRequired();

        positionObservation.Property(
                value => value.ObservedEntityId)
            .HasColumnName("observed_entity_id")
            .HasMaxLength(100)
            .IsRequired();

        positionObservation.Property(
                value => value.PositionFirstRecordedAt)
            .HasColumnName("position_first_recorded_at")
            .IsRequired();

        positionObservation.HasOne(
                value => value.Orb)
            .WithOne(
                value => value.PositionObservationOrb)
            .HasForeignKey<PositionObservationOrbRecord>(
                value => value.OrbId)
            .OnDelete(
                DeleteBehavior.Cascade);
    }

    private static void ConfigureSensoryOrbTargetEntity(
        ModelBuilder modelBuilder)
    {
        var target =
            modelBuilder.Entity<SensoryOrbTargetRecord>();

        target.ToTable(
            "sensory_orb_targets");

        target.HasKey(
            value => new
            {
                value.OrbId,
                value.TargetEntityId
            });

        target.Property(
                value => value.OrbId)
            .HasColumnName("orb_id")
            .IsRequired();

        target.Property(
                value => value.TargetEntityId)
            .HasColumnName("target_entity_id")
            .HasMaxLength(100)
            .IsRequired();

        target.HasOne(
                value => value.SensoryOrb)
            .WithMany(
                value => value.Targets)
            .HasForeignKey(
                value => value.OrbId)
            .OnDelete(
                DeleteBehavior.Cascade);
    }
}