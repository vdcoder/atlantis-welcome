using Microsoft.EntityFrameworkCore;
using Atlantis.Api.Data.Entities;

namespace Atlantis.Api.Data
{
    public class AtlantisDbContext : DbContext
    {
        public AtlantisDbContext(DbContextOptions<AtlantisDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorldEntity> Worlds => Set<WorldEntity>();
        public DbSet<EntityEntity> Entities => Set<EntityEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure WorldEntity
            modelBuilder.Entity<WorldEntity>()
                .HasKey(w => w.Id);

            modelBuilder.Entity<WorldEntity>()
                .HasIndex(w => w.WorldId)
                .IsUnique();

            modelBuilder.Entity<WorldEntity>()
                .Property(w => w.WorldId)
                .IsRequired()
                .HasMaxLength(100);

            // Configure EntityEntity
            modelBuilder.Entity<EntityEntity>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<EntityEntity>()
                .Property(e => e.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<EntityEntity>()
                .Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<EntityEntity>()
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<EntityEntity>()
                .Property(e => e.PlaceId)
                .IsRequired()
                .HasMaxLength(100);

            // Configure relationship
            modelBuilder.Entity<EntityEntity>()
                .HasOne(e => e.World)
                .WithMany(w => w.Entities)
                .HasForeignKey(e => e.WorldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EntityEntity>()
                .HasIndex(e => new { e.WorldId, e.EntityId })
                .IsUnique();
        }
    }
}
