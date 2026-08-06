using Atlantis.Api.Models;
using Atlantis.Api.Models.Orbs;
using Atlantis.Api.World.Orbs;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Feedback;

public sealed class CortexTaskFeedbackOrbService
{
    private readonly WorldOrbCollection
        _orbCollection;

    public CortexTaskFeedbackOrbService(
        WorldOrbCollection orbCollection)
    {
        _orbCollection =
            orbCollection ??
            throw new ArgumentNullException(
                nameof(orbCollection));
    }

    public Guid CreateInvalidCompletionFormatOrb(
        string workerCitizenId,
        string validationMessage,
        DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            validationMessage);

        var orb =
            new SensoryOrb(
                id:
                    Guid.NewGuid(),

                createdAt:
                    now,

                expiresAt:
                    now.AddSeconds(30),

                attachmentEntityId:
                    workerCitizenId,

                attachmentPosition:
                    new Position(
                        0f,
                        0f,
                        0f),

                radius:
                    0.5f,

                modality:
                    SensoryModality.Audio,

                content:
                    "Your Cortex task completion was not accepted. " +
                    validationMessage,

                intensity:
                    0.25f);

        _orbCollection.Add(
            orb);

        return orb.Id;
    }
}