namespace Atlantis.Api.World.EmbodiedControl;

public sealed class EmbodiedControllerRegistry
{
    private readonly Dictionary<
        string,
        Type>
        _controllerTypes =
            new(
                StringComparer.Ordinal);

    public void Register<TController>(
        string entityId)
        where TController :
            class,
            IEmbodiedController
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            entityId);

        if (!_controllerTypes.TryAdd(
                entityId,
                typeof(TController)))
        {
            throw new InvalidOperationException(
                $"A controller is already registered " +
                $"for entity '{entityId}'.");
        }
    }

    public IEmbodiedController ResolveRequired(
        string entityId,
        IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            entityId);

        ArgumentNullException.ThrowIfNull(
            serviceProvider);

        if (!_controllerTypes.TryGetValue(
                entityId,
                out var controllerType))
        {
            throw new InvalidOperationException(
                $"No embodied controller is registered " +
                $"for entity '{entityId}'.");
        }

        return
            (IEmbodiedController)
            serviceProvider.GetRequiredService(
                controllerType);
    }
}