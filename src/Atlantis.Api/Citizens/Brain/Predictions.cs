using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Brain
{
    public abstract record Prediction;

    public sealed record WaitPrediction : Prediction;

    public sealed record MovePrediction(
        float X,
        float Z)
        : Prediction;

    public sealed record SayPrediction(
        string Text)
        : Prediction;

    public sealed record TouchPrediction(
        string TargetQuery,
        string Text)
        : Prediction;
}
