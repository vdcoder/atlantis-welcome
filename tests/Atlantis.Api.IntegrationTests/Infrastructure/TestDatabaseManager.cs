using Npgsql;

namespace Atlantis.Api.IntegrationTests.Infrastructure;

internal sealed class TestDatabaseManager
{
    private readonly NpgsqlConnectionStringBuilder
        _administrativeConnection;

    public string TemplateConnectionString
    {
        get
        {
            var template =
                new NpgsqlConnectionStringBuilder(
                    _administrativeConnection.ConnectionString)
                {
                    Database =
                        TestDatabaseNames.Template
                };

            return template.ConnectionString;
        }
    }

    public TestDatabaseManager(
        string administrativeConnectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            administrativeConnectionString);

        _administrativeConnection =
            new NpgsqlConnectionStringBuilder(
                administrativeConnectionString);

        ValidateAdministrativeConnection();
    }

    public string ActualConnectionString
    {
        get
        {
            var actual =
                new NpgsqlConnectionStringBuilder(
                    _administrativeConnection.ConnectionString)
                {
                    Database =
                        TestDatabaseNames.Actual
                };

            return actual.ConnectionString;
        }
    }

    public async Task RecreateActualDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        await DropActualDatabaseAsync(
            cancellationToken);

        await using var connection =
            new NpgsqlConnection(
                _administrativeConnection.ConnectionString);

        await connection.OpenAsync(
            cancellationToken);

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            $"""
            CREATE DATABASE {QuoteIdentifier(
                TestDatabaseNames.Actual)}
            TEMPLATE {QuoteIdentifier(
                TestDatabaseNames.Template)};
            """;

        await command.ExecuteNonQueryAsync(
            cancellationToken);
    }

    public async Task DropActualDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        // Release connections retained by Npgsql after a
        // DbContext has been disposed.
        NpgsqlConnection.ClearAllPools();

        await using var connection =
            new NpgsqlConnection(
                _administrativeConnection.ConnectionString);

        await connection.OpenAsync(
            cancellationToken);

        await TerminateDatabaseConnectionsAsync(
            connection,
            TestDatabaseNames.Actual,
            cancellationToken);

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            $"""
            DROP DATABASE IF EXISTS {QuoteIdentifier(
                TestDatabaseNames.Actual)};
            """;

        await command.ExecuteNonQueryAsync(
            cancellationToken);
    }

    private static async Task
        TerminateDatabaseConnectionsAsync(
            NpgsqlConnection connection,
            string databaseName,
            CancellationToken cancellationToken)
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = @database_name
              AND pid <> pg_backend_pid();
            """;

        command.Parameters.AddWithValue(
            "database_name",
            databaseName);

        await command.ExecuteNonQueryAsync(
            cancellationToken);
    }

    private void ValidateAdministrativeConnection()
    {
        if (string.IsNullOrWhiteSpace(
            _administrativeConnection.Database))
        {
            throw new InvalidOperationException(
                "The administrative test connection must " +
                "specify a maintenance database such as " +
                "'postgres'.");
        }

        if (!string.Equals(
            _administrativeConnection.Database,
            "postgres",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The administrative test connection must " +
                "target the 'postgres' maintenance database.");
        }

        if (string.IsNullOrWhiteSpace(
                _administrativeConnection.Host))
        {
            throw new InvalidOperationException(
                "The administrative test connection must " +
                "specify a PostgreSQL host.");
        }

        if (string.IsNullOrWhiteSpace(
                _administrativeConnection.Username))
        {
            throw new InvalidOperationException(
                "The administrative test connection must " +
                "specify a PostgreSQL username.");
        }
    }

    private static string QuoteIdentifier(
        string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            identifier);

        return "\"" +
            identifier.Replace(
                "\"",
                "\"\"",
                StringComparison.Ordinal) +
            "\"";
    }
}