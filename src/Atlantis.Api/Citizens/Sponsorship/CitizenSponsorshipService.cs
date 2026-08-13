using System.Data;
using Atlantis.Api.Persistence;
using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Citizens.Sponsorship;

public sealed class CitizenSponsorshipService
{
    private readonly AtlantisDbContext _dbContext;

    private readonly WorldRuntime _worldRuntime;

    public CitizenSponsorshipService(
        AtlantisDbContext dbContext,
        WorldRuntime worldRuntime)
    {
        _dbContext =
            dbContext ??
            throw new ArgumentNullException(
                nameof(dbContext));

        _worldRuntime =
            worldRuntime ??
            throw new ArgumentNullException(
                nameof(worldRuntime));
    }

    public async Task<CitizenSponsorship>
        CreateAndActivateAsync(
            string citizenEntityId,
            string sponsorEntityId,
            string fundingAccountId,
            DateTimeOffset now,
            Guid? desiredId = null,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            citizenEntityId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            sponsorEntityId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            fundingAccountId);

        if (string.Equals(
                citizenEntityId,
                sponsorEntityId,
                StringComparison.Ordinal))
        {
            throw new CitizenSponsorshipValidationException(
                "A citizen cannot sponsor itself.");
        }

        var citizen =
            _worldRuntime.FindEntity(
                citizenEntityId)
            ?? throw new CitizenSponsorshipValidationException(
                $"Citizen entity '{citizenEntityId}' was not found.");

        if (!string.Equals(
                citizen.Type,
                "citizen",
                StringComparison.Ordinal))
        {
            throw new CitizenSponsorshipValidationException(
                $"Entity '{citizenEntityId}' is not a citizen.");
        }

        var sponsor =
            _worldRuntime.FindEntity(
                sponsorEntityId)
            ?? throw new CitizenSponsorshipValidationException(
                $"Sponsor entity '{sponsorEntityId}' was not found.");

        var fundingAccountExists =
            await _dbContext.MoneyAccounts
                .AsNoTracking()
                .AnyAsync(
                    account =>
                        account.Id ==
                            fundingAccountId &&
                        account.IsActive,
                    cancellationToken);

        if (!fundingAccountExists)
        {
            throw new CitizenSponsorshipValidationException(
                $"Active funding account '{fundingAccountId}' " +
                "was not found.");
        }

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

        var lockKey =
            $"citizen-sponsorship:{citizenEntityId}";

        try
        {
            await _dbContext.Database
                .ExecuteSqlInterpolatedAsync(
                    $"""
                    SELECT pg_advisory_xact_lock(
                        hashtextextended({lockKey}, 0)
                    )
                    """,
                    cancellationToken);

            var active =
                await _dbContext.CitizenSponsorships
                    .AsNoTracking()
                    .Where(entity =>
                        entity.CitizenEntityId ==
                            citizenEntityId &&
                        entity.Status ==
                            (int)CitizenSponsorshipStatus.Active)
                    .Select(entity => entity.Id)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (active != Guid.Empty)
            {
                throw new
                    ActiveCitizenSponsorshipExistsException(
                        citizenEntityId,
                        active);
            }

            var id =
                desiredId ??
                Guid.NewGuid();

            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Sponsorship ID cannot be empty.",
                    nameof(desiredId));
            }

            var entity =
                new CitizenSponsorshipRecord
                {
                    Id = id,

                    CitizenEntityId =
                        citizen.Id,

                    SponsorEntityId =
                        sponsor.Id,

                    FundingAccountId =
                        fundingAccountId,

                    Status =
                        (int)CitizenSponsorshipStatus.Active,

                    CreatedAt =
                        now,

                    ActivatedAt =
                        now
                };

            _dbContext.CitizenSponsorships.Add(
                entity);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return Map(entity);
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    public async Task<CitizenSponsorship?>
        GetActiveForCitizenAsync(
            string citizenEntityId,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            citizenEntityId);

        var entity =
            await _dbContext.CitizenSponsorships
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    sponsorship =>
                        sponsorship.CitizenEntityId ==
                            citizenEntityId &&
                        sponsorship.Status ==
                            (int)CitizenSponsorshipStatus.Active,
                    cancellationToken);

        return entity is null
            ? null
            : Map(entity);
    }

    public async Task<CitizenSponsorship>
        EndAsync(
            Guid sponsorshipId,
            string reason,
            DateTimeOffset now,
            CancellationToken cancellationToken = default)
    {
        return await EndCoreAsync(
            sponsorshipId,
            CitizenSponsorshipStatus.Ended,
            reason,
            now,
            cancellationToken);
    }

    public async Task<CitizenSponsorship>
        RemoveSponsorForViolationAsync(
            Guid sponsorshipId,
            string reason,
            DateTimeOffset now,
            CancellationToken cancellationToken = default)
    {
        return await EndCoreAsync(
            sponsorshipId,
            CitizenSponsorshipStatus
                .SponsorRemovedForViolation,
            reason,
            now,
            cancellationToken);
    }

    private async Task<CitizenSponsorship>
    EndCoreAsync(
        Guid sponsorshipId,
        CitizenSponsorshipStatus finalStatus,
        string reason,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (sponsorshipId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sponsorship ID cannot be empty.",
                nameof(sponsorshipId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            reason);

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

        try
        {
            var entity =
                await _dbContext.CitizenSponsorships
                    .FromSqlInterpolated(
                        $"""
                    SELECT *
                    FROM citizen_sponsorships
                    WHERE id = {sponsorshipId}
                    FOR UPDATE
                    """)
                    .SingleOrDefaultAsync(
                        cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"Sponsorship '{sponsorshipId}' was not found.");

            if (entity.Status !=
                (int)CitizenSponsorshipStatus.Active)
            {
                throw new InvalidOperationException(
                    $"Sponsorship '{sponsorshipId}' is not active.");
            }

            entity.Status =
                (int)finalStatus;

            entity.EndedAt =
                now;

            entity.EndReason =
                reason;

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return Map(entity);
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    private static CitizenSponsorship Map(
        CitizenSponsorshipRecord entity)
    {
        return new CitizenSponsorship
        {
            Id =
                entity.Id,

            CitizenEntityId =
                entity.CitizenEntityId,

            SponsorEntityId =
                entity.SponsorEntityId,

            FundingAccountId =
                entity.FundingAccountId,

            Status =
                (CitizenSponsorshipStatus)
                    entity.Status,

            CreatedAt =
                entity.CreatedAt,

            ActivatedAt =
                entity.ActivatedAt,

            EndedAt =
                entity.EndedAt,

            EndReason =
                entity.EndReason
        };
    }
}