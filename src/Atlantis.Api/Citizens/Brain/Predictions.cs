using Atlantis.Api.Common;

namespace Atlantis.Api.Citizens.Brain;

public abstract record Prediction;

public sealed record WaitPrediction
    : Prediction;

public sealed record MovePrediction(
    float X,
    float Z)
    : Prediction;

public sealed record TurnTorsoPrediction(
    Direction TorsoFront)
    : Prediction;

public sealed record SetGazePrediction(
    Direction GazeDirection)
    : Prediction;

public sealed record FaceAndLookPrediction(
    Direction Direction)
    : Prediction;

public sealed record SayPrediction(
    string Text)
    : Prediction;

public sealed record TouchPrediction(
    string TargetQuery,
    string Text)
    : Prediction;