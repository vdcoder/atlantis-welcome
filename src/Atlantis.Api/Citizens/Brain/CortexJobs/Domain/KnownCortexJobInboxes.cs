namespace Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

public static class KnownCortexJobInboxes
{
    public const string
        AtlantisDevelopmentSimulateCitizenPassCompletedInboxId =
            "inbox:atlantis-development:simulate-citizen-pass:completed";

    public const string
        AtlantisDevelopmentSimulateCitizenPassFailureInboxId =
            "inbox:atlantis-development:simulate-citizen-pass:failed";
}