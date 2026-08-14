using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.CortexContext;
using Atlantis.Api.Citizens.Brain.CortexTools;
using Atlantis.Api.World.EmbodiedControl;

namespace Atlantis.Api.Citizens.Control;

public sealed class CitizenEmbodiedController
    : ICitizenController
{
    private readonly CitizenCortexContextBuilder
        _contextBuilder;

    private readonly Predictor
        _predictor;

    public CitizenEmbodiedController(
        CitizenCortexContextBuilder contextBuilder,
        Predictor predictor)
    {
        _contextBuilder =
            contextBuilder ??
            throw new ArgumentNullException(
                nameof(contextBuilder));

        _predictor =
            predictor ??
            throw new ArgumentNullException(
                nameof(predictor));
    }

    public async Task<CitizenBreath>
        ProduceCitizenBreathAsync(
            EmbodiedControllerContext context,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            context);

        var entity =
            context.Entity;

        if (entity.Embodiment is null)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.Id}' is not embodied.");
        }

        var cortexContext =
            await _contextBuilder.BuildAsync(
                entity,
                context.NearbyEntities,
                context.AuditoryEvents,
                cancellationToken);

        var cognitiveOutput =
            await _predictor.PredictAsync(
                cortexContext,
                entity.Position,
                cancellationToken);

        if (cognitiveOutput is null)
        {
            throw new InvalidOperationException(
                $"Citizen cognition for '{entity.Id}' returned no output.");
        }

        if (cognitiveOutput.EmbodiedPredictions is null)
        {
            throw new InvalidOperationException(
                $"Citizen cognition for '{entity.Id}' returned a null " +
                "embodied prediction list.");
        }

        if (cognitiveOutput.CortexToolCalls is null)
        {
            throw new InvalidOperationException(
                $"Citizen cognition for '{entity.Id}' returned a null " +
                "Cortex tool-call list.");
        }

        return new CitizenBreath
        {
            EmbodiedPredictions =
                new PredictionBatch
                {
                    EntityId =
                        entity.Id,

                    BasedOnWorldRevision =
                        context.ObservedWorldRevision,

                    ProducedAt =
                        DateTimeOffset.UtcNow,

                    Predictions =
                        cognitiveOutput
                            .EmbodiedPredictions
                },

            CortexToolCalls =
                cognitiveOutput.CortexToolCalls
        };
    }
}