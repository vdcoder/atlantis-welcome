using Atlantis.Api.Citizens.AgentKernel;
using Atlantis.Api.Citizens.Brain;
using Atlantis.Api.Citizens.Brain.BrainTools;
using Atlantis.Api.Citizens.Interaction;
using Atlantis.Api.Citizens.Perception;
using Atlantis.Api.Models;
using Atlantis.Api.World;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Runtime
{
    public sealed class CitizenRuntime
    {
        private readonly WorldRuntime _worldRuntime;
        private readonly MoveTo _moveTool;
        private readonly Say _sayTool;
        private readonly Touch _touchTool;
        private readonly NearbyEntityPerception _perception;
        private readonly Predictor _predictor;
        private readonly SemanticReach _semanticReach;
        private readonly ILogger<CitizenRuntime> _logger;

        public CitizenRuntime(
            WorldRuntime worldRuntime,
            MoveTo moveTool,
            Say sayTool,
            Touch touchTool,
            NearbyEntityPerception perception,
            Predictor predictor,
            SemanticReach semanticReach,
            ILogger<CitizenRuntime> logger)
        {
            _worldRuntime = worldRuntime;
            _moveTool = moveTool;
            _sayTool = sayTool;
            _touchTool = touchTool;
            _perception = perception;
            _predictor = predictor;
            _semanticReach = semanticReach;
            _logger = logger;
        }

        public async Task RunOneIterationAsync(
            string citizenId,
            CancellationToken cancellationToken = default)
        {
            var snapshot = _worldRuntime.GetSnapshot();

            var citizen = snapshot.World.Entities
                .FirstOrDefault(entity =>
                    entity.Id == citizenId);

            if (citizen == null)
            {
                return;
            }

            var nearbyObjects =
                _perception.Perceive(
                    citizen,
                    snapshot.World);

            var context = BuildContext(
                citizen,
                nearbyObjects);

            var prediction =
                _predictor.Predict(
                    context,
                    citizen.Position);
            _logger.LogInformation(
                "Citizen {CitizenId} predicted {Prediction}",
                citizen.Id,
                prediction);

            await ExecutePredictionAsync(
                citizen,
                context,
                prediction,
                cancellationToken);
        }

        private async Task ExecutePredictionAsync(
            Entity citizen,
            Context context,
            Prediction prediction,
            CancellationToken cancellationToken = default)
        {
            switch (prediction)
            {
                case WaitPrediction:
                    // Do nothing
                    break;
                case MovePrediction move:
                    await _moveTool.MoveToAsync(
                        citizen.Id,
                        move.X,
                        move.Z);
                    break;
                case SayPrediction speak:
                    await _sayTool.SayAsync(
                        citizen.Id,
                        speak.Text);
                    break;
                case TouchPrediction touch:
                    {
                        var match = _semanticReach.Resolve(
                            touch.TargetQuery,
                            context.NearbyObjects);

                        if (match == null)
                        {
                            _logger.LogWarning(
                                "Citizen {CitizenId} failed to resolve target for touch action: {TargetQuery}",
                                citizen.Id,
                                touch.TargetQuery);
                            return;
                        }

                        _logger.LogInformation(
                            "Semantic Reach resolved '{Query}' to " +
                            "{TargetId} at {Distance:F2}m with score {Score}",
                            touch.TargetQuery,
                            match.EntityId,
                            match.Distance,
                            match.Score);

                        await _touchTool.TouchAsync(
                            citizen.Id,
                            match.EntityId,
                            touch.Text);
                    } break;
            }
        }

        private static Context BuildContext(
            Entity citizen,
            IReadOnlyList<PerceivedObject> nearbyObjects)
        {
            return new Context
            {
                CitizenId = citizen.Id,

                StaticContext =
                    $"You are {citizen.Name}, a citizen of Atlantis.",

                DynamicContext =
                    $"You are at ({citizen.Position.X:F1}, " +
                    $"{citizen.Position.Y:F1}, " +
                    $"{citizen.Position.Z:F1}).",

                NearbyObjects = nearbyObjects,

                Memory = string.Empty,
                RecentHistory = string.Empty
            };
        }
    }
}
