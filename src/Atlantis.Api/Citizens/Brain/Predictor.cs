using Atlantis.Api.Citizens.AgentKernel;
using Atlantis.Api.Models;
using Microsoft.Build.Tasks;

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

        public Prediction Predict(Context context, Position currentPosition)
        {
            return new TouchPrediction(
        TargetQuery: "visitor",
        Text: "Hello privately.");

            var value = Random.Shared.Next(100);

            if (value < 30) // Do nothing / wait
            {
                return new WaitPrediction();
            }

            if (value < 30 + 25)
            {
                var offsetX =
                    Random.Shared.NextSingle() * 4f - 2f;

                var offsetZ =
                    Random.Shared.NextSingle() * 4f - 2f;

                return new MovePrediction(
                    currentPosition.X + offsetX,
                    currentPosition.Z + offsetZ);
            }

            if (value < 30 + 25 + 25)
            {
                return new SayPrediction(
                Phrases[Random.Shared.Next(Phrases.Length)]);
            }

            if (context.NearbyObjects.Count > 0)
            {
                var target =
                    context.NearbyObjects[
                        Random.Shared.Next(
                            context.NearbyObjects.Count)];

                return new TouchPrediction(
                    TargetQuery: target.Type,
                    Text: "Hello privately.");
            }

            return new WaitPrediction();
        }
    }
}
