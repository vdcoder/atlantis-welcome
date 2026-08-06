namespace Atlantis.Api.Citizens.Control;

public sealed class CitizenControllerRegistry
{
    private readonly Dictionary<string, Type>
        _controllerTypes =
            new(StringComparer.Ordinal);

    public void Register<TController>(
        string citizenId)
        where TController :
            class,
            ICitizenController
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            citizenId);

        if (!_controllerTypes.TryAdd(
                citizenId,
                typeof(TController)))
        {
            throw new InvalidOperationException(
                $"A citizen controller is already registered " +
                $"for citizen '{citizenId}'.");
        }
    }

    public ICitizenController ResolveRequired(
        string citizenId,
        IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            citizenId);

        ArgumentNullException.ThrowIfNull(
            serviceProvider);

        if (!_controllerTypes.TryGetValue(
                citizenId,
                out var controllerType))
        {
            throw new InvalidOperationException(
                $"No citizen controller is registered " +
                $"for citizen '{citizenId}'.");
        }

        return
            (ICitizenController)
            serviceProvider.GetRequiredService(
                controllerType);
    }
}