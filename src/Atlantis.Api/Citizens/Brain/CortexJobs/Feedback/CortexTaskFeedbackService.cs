using Atlantis.Api.World.Actions;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Feedback;

public sealed class CortexTaskFeedbackService
{
    private readonly WorldActionProcessor
        _actionProcessor;

    public CortexTaskFeedbackService(
        WorldActionProcessor actionProcessor)
    {
        _actionProcessor =
            actionProcessor ??
            throw new ArgumentNullException(
                nameof(actionProcessor));
    }

    public async Task<Guid>
        CreateInvalidCompletionFormatSayAsync(
            string workerCitizenId,
            string validationMessage,
            DateTimeOffset now,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            validationMessage);

        var request =
            new SayRequest(
                ActorId:
                    null,

                EntityId:
                    workerCitizenId,

                Text:
                    "Your Cortex task completion was not accepted. " +
                    validationMessage,

                SpokenAt: now);

        var transitions =
            await _actionProcessor.ProcessAsync(
                request);

        var created =
            transitions
                .OfType<SensoryOrbCreatedTransition>()
                .Single();

        return created.Orb.Id;
    }
}