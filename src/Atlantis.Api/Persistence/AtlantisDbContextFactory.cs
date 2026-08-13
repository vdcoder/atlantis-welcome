using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Atlantis.Api.Persistence;

public sealed class AtlantisDbContextFactory
    : IDesignTimeDbContextFactory<AtlantisDbContext>
{
    public AtlantisDbContext CreateDbContext(
        string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(
                    Directory.GetCurrentDirectory())
                .AddJsonFile(
                    "appsettings.json",
                    optional:
                        false)
                .AddJsonFile(
                    $"appsettings.{environment}.json",
                    optional:
                        true)
                .AddEnvironmentVariables()
                .Build();

        var connectionString =
            configuration.GetConnectionString(
                "Atlantis");

        if (string.IsNullOrWhiteSpace(
                connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Atlantis' " +
                "is not configured for EF design-time.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<
                AtlantisDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new AtlantisDbContext(
            optionsBuilder.Options);
    }
}
