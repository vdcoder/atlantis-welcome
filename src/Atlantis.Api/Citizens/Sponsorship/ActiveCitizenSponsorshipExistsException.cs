namespace Atlantis.Api.Citizens.Sponsorship;

public sealed class ActiveCitizenSponsorshipExistsException
    : Exception
{
    public ActiveCitizenSponsorshipExistsException(
        string citizenEntityId,
        Guid sponsorshipId)
        : base(
            $"Citizen '{citizenEntityId}' already has " +
            $"active sponsorship '{sponsorshipId}'.")
    {
        CitizenEntityId = citizenEntityId;
        SponsorshipId = sponsorshipId;
    }

    public string CitizenEntityId { get; }

    public Guid SponsorshipId { get; }
}