namespace Atlantis.Api.IntegrationTests.Infrastructure;

internal static class TestDatabaseEnvironment
{
    private const string
        AdministrativeConnectionVariable =
            "ATLANTIS_TEST_ADMIN_CONNECTION";

    public static TestDatabaseManager
        CreateManager()
    {
        var administrativeConnectionString =
            Environment.GetEnvironmentVariable(
                AdministrativeConnectionVariable);

        if (string.IsNullOrWhiteSpace(
                administrativeConnectionString))
        {
            throw new InvalidOperationException(
                $"Environment variable " +
                $"'{AdministrativeConnectionVariable}' " +
                $"is not configured.");
        }

        return new TestDatabaseManager(
            administrativeConnectionString);
    }
}