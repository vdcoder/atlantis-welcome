using Atlantis.Api.Citizens.Brain;

namespace Atlantis.Api.World.EmbodiedControl;

public sealed record PredictionBatch
{
    public required string EntityId { get; init; }

    public required long BasedOnWorldRevision { get; init; }

    public required DateTimeOffset ProducedAt { get; init; }

    public required IReadOnlyList<Prediction>
        Predictions
    {
        get;
        init;
    }
}