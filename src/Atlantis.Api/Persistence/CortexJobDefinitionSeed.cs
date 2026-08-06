using Atlantis.Api.Data;
using Atlantis.Api.Economy.Accounts;
using Atlantis.Api.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Atlantis.Api.Citizens.Brain.CortexJobs.Domain;

namespace Atlantis.Api.Persistence
{
    public static class CortexJobDefinitionSeed
    {
        public static async Task EnsureSeededAsync(
            AtlantisDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            var exists =
                await dbContext.CortexJobDefinitions
                    .AnyAsync(
                        definition =>
                            definition.Id ==
                            KnownCortexJobDefinitions
                                .AtlantisDevelopmentSimulateCitizenPassId,
                        cancellationToken);

            if (exists)
            {
                return;
            }

            var definition = new CortexJobDefinition
            {
                Id =
                    KnownCortexJobDefinitions
                        .AtlantisDevelopmentSimulateCitizenPassId,

                EmployerAccountId =
                    KnownMoneyAccounts
                        .AtlantisDevelopmentFundId,

                Name = "Simulate Citizen Pass",

                Description =
                    "Produce a list of predictions for one " +
                    "externally supplied simulated citizen pass.",

                Qualifications =
                    "Approved development simulation worker.",

                Schedule = new CortexJobSchedule(),

                CompletionInboxId =
                    KnownCortexJobInboxes
                        .AtlantisDevelopmentSimulateCitizenPassCompletedInboxId,

                FailureInboxId =
                    KnownCortexJobInboxes
                        .AtlantisDevelopmentSimulateCitizenPassFailureInboxId,

                CreatedAt = DateTimeOffset.UtcNow,

                DeactivatedAt = null,
            };

            dbContext.CortexJobDefinitions.Add(
                CortexJobDefinitionMapper.ToEntity(
                    definition));

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}
