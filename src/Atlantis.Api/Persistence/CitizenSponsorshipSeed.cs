using Atlantis.Api.Citizens.Sponsorship;
using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public static class CitizenSponsorshipSeed
{
    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CitizenSponsorshipService sponsorshipService,
        CancellationToken cancellationToken = default)
    {
        var id =
            KnownCitizenSponsorships
                .OrestesVictorSponsorshipId;

        var exists =
            await dbContext.CitizenSponsorships
                .AnyAsync(
                    entity =>
                        entity.Id == id,
                    cancellationToken);

        if (exists)
        {
            return;
        }

        await sponsorshipService
            .CreateAndActivateAsync(
                citizenEntityId:
                    "orestes",

                sponsorEntityId:
                    "human:victor",

                fundingAccountId:
                    KnownMoneyAccounts
                        .AtlantisDevelopmentFundId,

                now:
                    DateTimeOffset.UtcNow,

                desiredId:
                    id,

                cancellationToken:
                    cancellationToken);
    }
}