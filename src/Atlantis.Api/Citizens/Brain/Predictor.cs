using Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;
using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;
using Atlantis.Api.Citizens.Brain.CortexTools;
using Atlantis.Api.Citizens.Brain.CortexJobs.Simulation;
using Atlantis.Api.Models;

namespace Atlantis.Api.Citizens.Brain
{
    public sealed class Predictor
    {
        private static readonly string[] Phrases =
        [
            "Hello from Atlantis.",
            "I am still here.",
            "This world is beginning to take shape.",
            "I wonder what lies beyond the box.",
            "A visitor is nearby."
        ];

        private readonly
            SimulateCitizenPassDevelopmentResultFactory
            _simulationResultFactory;

        public Predictor(
            SimulateCitizenPassDevelopmentResultFactory simulationResultFactory)
        {
            _simulationResultFactory = simulationResultFactory ??
                throw new ArgumentNullException(
                    nameof(simulationResultFactory));
        }

        public Task<CitizenCognitiveOutput> PredictAsync(
            CortexContext.CortexContext context,
            Position currentPosition,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(
                context);

            ArgumentNullException.ThrowIfNull(
                currentPosition);

            cancellationToken
                .ThrowIfCancellationRequested();


            var primedTaskGenerator =
                context.DynamicContext
                    .FindGenerator<
                        PrimedCortexTaskContextGenerator>();

            var primedTask =
                primedTaskGenerator?.GeneratedTask;

            if (primedTask is not null &&
                string.Equals(
                    primedTask.CortexTaskType,
                    CortexTaskResultTypes
                        .SimulateCitizenPass,
                    StringComparison.Ordinal))
            {
                return Task.FromResult(
                    new CitizenCognitiveOutput
                    {
                        EmbodiedPredictions =
                        [
                            new WaitPrediction()
                        ],

                        CortexToolCalls =
                        [
                            new CompleteCortexTaskCall
                            {
                                AssignmentId =
                                    primedTask.AssignmentId,

                                ResultSerialized =
                                    _simulationResultFactory
                                        .CreateSerializedResult(
                                            primedTask)
                            }
                        ]
                    });
            }


            var value =
                Random.Shared.Next(100);

            if (value < 25)
            {
                return Task.FromResult(
                    Output(
                        new WaitPrediction()));
            }

            if (value < 45)
            {
                var offsetX =
                    Random.Shared.NextSingle() * 4f - 2f;

                var offsetZ =
                    Random.Shared.NextSingle() * 4f - 2f;

                return Task.FromResult(
                    Output(
                        new MovePrediction(
                            currentPosition.X + offsetX,
                            currentPosition.Z + offsetZ)));
            }

            if (value < 60)
            {
                var angle =
                    Random.Shared.NextSingle() *
                    MathF.Tau;

                return Task.FromResult(
                    Output(
                        new FaceAndLookPrediction(
                            new Direction(
                                MathF.Sin(angle),
                                0f,
                                MathF.Cos(angle)))));
            }

            if (value < 80)
            {
                return Task.FromResult(
                    Output(
                        new SayPrediction(
                            Phrases[
                                Random.Shared.Next(
                                    Phrases.Length)])));
            }

            return Task.FromResult(
                Output(
                    new WaitPrediction()));
        }

        private static CitizenCognitiveOutput Output(
            params Prediction[] predictions)
        {
            return new CitizenCognitiveOutput
            {
                EmbodiedPredictions =
                    predictions,

                CortexToolCalls =
                    Array.Empty<CortexToolCall>()
            };
        }
    }
}
