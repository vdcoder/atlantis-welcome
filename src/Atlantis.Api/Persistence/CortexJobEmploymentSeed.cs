using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;
using Atlantis.Api.Citizens.Brain.CortexJobs.Onboarding;
using Atlantis.Api.Citizens.Brain.CortexJobs.Qualifications;
using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence;

public static class CortexJobEmploymentSeed
{
    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var applicationId =
            KnownCortexJobEmployment
                .OrestesSimulateCitizenPassApplicationId;

        var qualificationId =
            KnownCortexJobEmployment
                .OrestesSimulateCitizenPassQualificationId;

        var jobDefinitionId =
            KnownCortexJobDefinitions
                .AtlantisDevelopmentSimulateCitizenPassId;

        var now = DateTimeOffset.UtcNow;

        var application =
            await dbContext.CortexJobApplications
                .SingleOrDefaultAsync(
                    entity =>
                        entity.Id == applicationId,
                    cancellationToken);

        if (application is null)
        {
            application = new CortexJobApplicationEntity
            {
                Id = applicationId,
                CortexJobDefinitionId =
                    jobDefinitionId,
                WorkerCitizenId = "orestes",
                DepositAccountId =
                    KnownMoneyAccounts.OrestesId,
                Status =
                    (int)CortexJobApplicationStatus
                        .Approved,
                AppliedAt = now,
                ApprovedAt = now
            };

            dbContext.CortexJobApplications.Add(
                application);

            // Persist application first because qualification
            // references it.
            await dbContext.SaveChangesAsync(
                cancellationToken);
        }

        var qualificationExists =
            await dbContext
                .WorkerCortexJobQualifications
                .AnyAsync(
                    entity =>
                        entity.Id == qualificationId,
                    cancellationToken);

        if (!qualificationExists)
        {
            dbContext
                .WorkerCortexJobQualifications
                .Add(
                    new WorkerCortexJobQualificationEntity
                    {
                        Id = qualificationId,
                        CortexJobApplicationId =
                            applicationId,
                        WorkerCitizenId = "orestes",
                        DepositAccountId =
                            KnownMoneyAccounts.OrestesId,
                        CortexJobDefinitionId =
                            jobDefinitionId,
                        Status =
                            (int)
                            WorkerCortexJobQualificationStatus
                                .Approved,
                        QualifiedAt =
                            application.ApprovedAt ??
                            now
                    });

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }

        var availabilityExists =
            await dbContext
                .WorkerCortexJobAvailability
                .AnyAsync(
                    entity =>
                        entity
                            .WorkerCortexJobQualificationId ==
                        qualificationId,
                    cancellationToken);

        if (!availabilityExists)
        {
            dbContext
                .WorkerCortexJobAvailability
                .Add(
                    new WorkerCortexJobAvailabilityEntity
                    {
                        WorkerCortexJobQualificationId =
                            qualificationId,
                        IsAvailable = false,
                        ChangedAt = now
                    });

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}