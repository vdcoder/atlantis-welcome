using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Atlantis.Api.Data
{
    public class AtlantisDbContextFactory : IDesignTimeDbContextFactory<AtlantisDbContext>
    {
        public AtlantisDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AtlantisDbContext>();

            // Use environment variable or default connection string
            //var connectionString = Environment.GetEnvironmentVariable("ATLANTIS_DB_CONNECTION")
            //    ?? "Host=localhost;Port=5432;Database=atlantis;Username=atlantis;Password=atlantis-dev-password";

            var connectionString = "Host=localhost;Port=5432;Database=atlantis;Username=atlantis;Password=atlantis-dev-password";

            optionsBuilder.UseNpgsql(connectionString);

            return new AtlantisDbContext(optionsBuilder.Options);
        }
    }
}
