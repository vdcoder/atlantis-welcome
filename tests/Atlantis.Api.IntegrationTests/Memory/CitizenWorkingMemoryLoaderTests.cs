using Atlantis.Api.Citizens.Brain.Memory.WorkingMemory;
using Atlantis.Api.IntegrationTests.Infrastructure;
using Atlantis.Api.Persistence.Records;
using Atlantis.Api.Persistence.Services;
using Xunit;

namespace Atlantis.Api.IntegrationTests.Memory
{
    public sealed class CitizenWorkingMemoryLoaderTests
        : IsolatedDatabaseTest
    {
        [Fact]
        public async Task
            LoadAsync_ReconstructsLatestLinesAndBoundedStream()
        {
            await using var dbContext =
                CreateDbContext();

            var citizenId =
                "orestes";

            var baseTime =
                new DateTimeOffset(
                    2026,
                    8,
                    15,
                    14,
                    0,
                    0,
                    TimeSpan.Zero);

            dbContext.CitizenWorkingMemoryLines.AddRange(
                new CitizenWorkingMemoryLineRecord
                {
                    Id =
                        Guid.NewGuid(),

                    CitizenId =
                        citizenId,

                    LineNumber =
                        1,

                    Content =
                        "old line one",

                    ContentTokenCount =
                        3,

                    CreatedAt =
                        baseTime
                },

                new CitizenWorkingMemoryLineRecord
                {
                    Id =
                        Guid.NewGuid(),

                    CitizenId =
                        citizenId,

                    LineNumber =
                        1,

                    Content =
                        "new line one",

                    ContentTokenCount =
                        3,

                    CreatedAt =
                        baseTime.AddSeconds(1)
                },

                new CitizenWorkingMemoryLineRecord
                {
                    Id =
                        Guid.NewGuid(),

                    CitizenId =
                        citizenId,

                    LineNumber =
                        22,

                    Content =
                        "dynamic line",

                    ContentTokenCount =
                        2,

                    CreatedAt =
                        baseTime.AddSeconds(2)
                });

            dbContext.CitizenMemoryStreamEntries.AddRange(
                CreateStreamEntry(
                    citizenId,
                    "first",
                    300,
                    baseTime.AddSeconds(10)),

                CreateStreamEntry(
                    citizenId,
                    "second",
                    300,
                    baseTime.AddSeconds(11)),

                CreateStreamEntry(
                    citizenId,
                    "third",
                    300,
                    baseTime.AddSeconds(12)),

                CreateStreamEntry(
                    citizenId,
                    "fourth",
                    800,
                    baseTime.AddSeconds(13)));

            await dbContext.SaveChangesAsync();

            var loader =
                new CitizenWorkingMemoryLoader(
                    dbContext);

            var memory =
                await loader.LoadAsync(
                    citizenId);

            Assert.Equal(
                "new line one",
                memory.GetLineContent(1));

            Assert.Equal(
                "dynamic line",
                memory.GetLineContent(22));

            Assert.Equal(
                string.Empty,
                memory.GetLineContent(40));

            Assert.Single(
                memory.Stream);

            Assert.Equal(
                "fourth",
                memory.Stream[0].Content);

            Assert.Equal(
                800 +
                WorkingMemoryLimits.StreamEntryHeaderTokens,
                memory.StreamTokenCount);
        }

        private static CitizenMemoryStreamEntryRecord
            CreateStreamEntry(
                string citizenId,
                string content,
                int contentTokenCount,
                DateTimeOffset createdAt)
        {
            return new CitizenMemoryStreamEntryRecord
            {
                Id =
                    Guid.NewGuid(),

                CitizenId =
                    citizenId,

                Content =
                    content,

                ContentTokenCount =
                    contentTokenCount,

                CreatedAt =
                    createdAt
            };
        }
    }
}