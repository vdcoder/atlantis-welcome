using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Control.EmbodiedTools;
using Atlantis.Api.Citizens.Interaction;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.World.Entities;

namespace Atlantis.Api.Citizens.Control;

public sealed class EmbodiedPredictionExecutor
{
    private readonly MoveTo _moveTool;
    private readonly Say _sayTool;
    private readonly Touch _touchTool;
    private readonly TurnTorso _turnTorsoTool;
    private readonly SetGaze _setGazeTool;
    private readonly FaceAndLook _faceAndLookTool;
    private readonly SemanticReach _semanticReach;
    private readonly ILogger<EmbodiedPredictionExecutor>
        _logger;

    public EmbodiedPredictionExecutor(
        MoveTo moveTool,
        Say sayTool,
        Touch touchTool,
        TurnTorso turnTorsoTool,
        SetGaze setGazeTool,
        FaceAndLook faceAndLookTool,
        SemanticReach semanticReach,
        ILogger<EmbodiedPredictionExecutor> logger)
    {
        _moveTool = moveTool;
        _sayTool = sayTool;
        _touchTool = touchTool;
        _turnTorsoTool = turnTorsoTool;
        _setGazeTool = setGazeTool;
        _faceAndLookTool = faceAndLookTool;
        _semanticReach = semanticReach;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Entity entity,
        IReadOnlyList<PerceivedEntityBinding> nearbyEntityBindings,
        Prediction prediction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(nearbyEntityBindings);
        ArgumentNullException.ThrowIfNull(prediction);

        switch (prediction)
        {
            case WaitPrediction:
                return;

            case MovePrediction move:
                await _moveTool.MoveToAsync(
                    entity.Id,
                    move.X,
                    move.Z);
                return;

            case SayPrediction say:
                await _sayTool.SayAsync(
                    entity.Id,
                    say.Text);
                return;

            case TouchPrediction touch:
                await ProcessTouchPredictionAsync(
                    entity,
                    nearbyEntityBindings,
                    touch);
                return;

            case TurnTorsoPrediction turn:
                await _turnTorsoTool.TurnTorsoAsync(
                    entity.Id,
                    turn.TorsoFront);
                return;

            case SetGazePrediction gaze:
                await _setGazeTool.SetGazeAsync(
                    entity.Id,
                    gaze.GazeDirection);
                return;

            case FaceAndLookPrediction faceAndLook:
                await _faceAndLookTool.FaceAndLookAsync(
                    entity.Id,
                    faceAndLook.Direction);
                return;

            default:
                throw new NotSupportedException(
                    $"Unsupported prediction type " +
                    $"'{prediction.GetType().Name}'.");
        }
    }

    private async Task ProcessTouchPredictionAsync(
            Entity entity,
            IReadOnlyList<PerceivedEntityBinding> nearbyEntityBindings,
            TouchPrediction prediction)
    {
        var resolution =
            _semanticReach.Resolve(
                prediction.TargetQuery,
                nearbyEntityBindings);

        if (resolution.Status !=
            TargetResolutionStatus.Found)
        {
            _logger.LogWarning(
                "Entity {EntityId} could not uniquely resolve touch target " +
                "'{TargetQuery}'. Resolution={Resolution}.",
                entity.Id,
                prediction.TargetQuery,
                resolution.Status);

            return;
        }

        var match =
            resolution.Match!;

        _logger.LogInformation(
            "Semantic Reach resolved '{Query}' to {TargetId} " +
            "via '{Reference}' at {Distance:F2}m.",
            prediction.TargetQuery,
            match.EntityId,
            match.Reference,
            match.Distance);

        await _touchTool.TouchAsync(
            entity.Id,
            match.EntityId,
            prediction.Text);
    }
}