namespace Atlantis.Api.Citizens.Sponsorship;

public sealed class CitizenSponsorshipValidationException
    : Exception
{
    public CitizenSponsorshipValidationException(
        string message)
        : base(message)
    {
    }
}