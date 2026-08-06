using CStaticContext = Atlantis.Api.Citizens.Brain.CortexContext.StaticContext.StaticContext;
using CDynamicContext = Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.DynamicContext;
using CMemoryContext = Atlantis.Api.Citizens.Brain.CortexContext.MemoryContext.MemoryContext;
using CEchoContext = Atlantis.Api.Citizens.Brain.CortexContext.EchoContext.EchoContext;
using Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;
using Atlantis.Api.Citizens.Brain.CortexContext.StaticContext.Generators;
using Atlantis.Api.Citizens.Brain.CortexJobs.Context;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Models;

namespace Atlantis.Api.Citizens.Brain.CortexContext;

public sealed class CitizenCortexContextBuilder
{
    private readonly IPrimedCortexTaskContextLoader
        _primedCortexTaskContextLoader;

    public CitizenCortexContextBuilder(
        IPrimedCortexTaskContextLoader
            primedCortexTaskContextLoader)
    {
        _primedCortexTaskContextLoader =
            primedCortexTaskContextLoader ??
            throw new ArgumentNullException(
                nameof(primedCortexTaskContextLoader));
    }

    public async Task<CortexContext> BuildAsync(
        Entity citizen,
        IReadOnlyList<PerceivedObject> nearbyObjects,
        IReadOnlyList<PerceivedSensoryOrb> sensoryOrbs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            citizen);

        ArgumentNullException.ThrowIfNull(
            nearbyObjects);

        ArgumentNullException.ThrowIfNull(
            sensoryOrbs);

        if (citizen.Embodiment is null)
        {
            throw new InvalidOperationException(
                $"Citizen '{citizen.Id}' is not embodied.");
        }

        var staticContext =
            new CStaticContext(
            [
                new AtlantisSystemContextGenerator()
            ]);

        var dynamicContext =
            new CDynamicContext(
            [
                new CurrentPositionContextGenerator(
                    citizen.Position),

                new NearbyObjectsContextGenerator(
                    nearbyObjects),

                new SensoryOrbContextGenerator(
                    sensoryOrbs),

                new PrimedCortexTaskContextGenerator(
                    citizen.Id,
                    _primedCortexTaskContextLoader)
            ]);

        // Generate once now so database-backed generators freeze their
        // observations for this breath.
        await staticContext.GenerateAsync(
            cancellationToken);

        await dynamicContext.GenerateAsync(
            cancellationToken);

        return new CortexContext
        {
            StaticContext =
                staticContext,

            DynamicContext =
                dynamicContext,

            Memory =
                new CMemoryContext(),

            Echo =
                new CEchoContext()
        };
    }
}