using Atlantis.Api.Citizens.Sponsorship;
using Microsoft.AspNetCore.Mvc;

namespace Atlantis.Api.Controllers;

[ApiController]
[Route("api/citizens/{citizenEntityId}/sponsorship")]
public sealed class CitizenSponsorshipController
    : ControllerBase
{
    private readonly CitizenSponsorshipService
        _sponsorshipService;

    public CitizenSponsorshipController(
        CitizenSponsorshipService sponsorshipService)
    {
        _sponsorshipService =
            sponsorshipService;
    }

    [HttpGet("active")]
    public async Task<ActionResult<CitizenSponsorship>>
        GetActiveAsync(
            string citizenEntityId,
            CancellationToken cancellationToken)
    {
        var sponsorship =
            await _sponsorshipService
                .GetActiveForCitizenAsync(
                    citizenEntityId,
                    cancellationToken);

        if (sponsorship is null)
        {
            return NotFound();
        }

        return Ok(sponsorship);
    }
}