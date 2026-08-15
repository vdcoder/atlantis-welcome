using Atlantis.Api.Citizens.Brain.Memory.WorkingMemory;
using Xunit;

namespace Atlantis.Api.IntegrationTests.Memory
{
    public sealed class CitizenWorkingMemoryTests
    {
        [Fact]
        public void GetLineContent_ReturnsEmptyForUnwrittenLine()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Equal(
                string.Empty,
                memory.GetLineContent(1));

            Assert.Equal(
                string.Empty,
                memory.GetLineContent(
                    WorkingMemoryLimits.MaxLines));
        }

        [Fact]
        public void GetLineContent_RejectsLineBelowRange()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                    () =>
                        memory.GetLineContent(0));
        }

        [Fact]
        public void GetLineContent_RejectsLineAboveRange()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                    () =>
                        memory.GetLineContent(
                            WorkingMemoryLimits.MaxLines +
                            1));
        }

        [Fact]
        public void Lines_HasConfiguredNumberOfSlots()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Equal(
                WorkingMemoryLimits.MaxLines,
                memory.Lines.Count);
        }

        [Fact]
        public void WorkingMemoryLimits_DefinesStaticAndDynamicTiers()
        {
            Assert.Equal(
                20,
                WorkingMemoryLimits.StaticLineCount);

            Assert.Equal(
                20,
                WorkingMemoryLimits.DynamicLineCount);

            Assert.Equal(
                WorkingMemoryLimits.StaticLineCount +
                WorkingMemoryLimits.DynamicLineCount,
                WorkingMemoryLimits.MaxLines);
        }

        [Fact]
        public void AppendToStream_AppendsEntriesInOrder()
        {
            var memory =
                CreateWorkingMemory();

            var firstAt =
                DateTimeOffset.UtcNow;

            var secondAt =
                firstAt.AddSeconds(1);

            memory.AppendToStream(
                "first",
                10,
                firstAt);

            memory.AppendToStream(
                "second",
                20,
                secondAt);

            Assert.Equal(
                2,
                memory.Stream.Count);

            Assert.Equal(
                "first",
                memory.Stream[0].Content);

            Assert.Equal(
                firstAt,
                memory.Stream[0].CreatedAt);

            Assert.Equal(
                "second",
                memory.Stream[1].Content);

            Assert.Equal(
                secondAt,
                memory.Stream[1].CreatedAt);
        }

        [Fact]
        public void AppendToStream_TokenCountIncludesEntryHeader()
        {
            var memory =
                CreateWorkingMemory();

            memory.AppendToStream(
                "hello",
                10,
                DateTimeOffset.UtcNow);

            Assert.Equal(
                10 +
                WorkingMemoryLimits.StreamEntryHeaderTokens,
                memory.StreamTokenCount);
        }

        [Fact]
        public void AppendToStream_UnderBudget_DoesNotEvict()
        {
            var memory =
                CreateWorkingMemory();

            memory.AppendToStream(
                "first",
                100,
                DateTimeOffset.UtcNow);

            memory.AppendToStream(
                "second",
                100,
                DateTimeOffset.UtcNow);

            Assert.Equal(
                2,
                memory.Stream.Count);
        }

        [Fact]
        public void AppendToStream_Overflow_EvictsOldestWholeEntry()
        {
            var memory =
                CreateWorkingMemory();

            var firstContentTokens =
                600;

            var secondContentTokens =
                500;

            memory.AppendToStream(
                "first",
                firstContentTokens,
                DateTimeOffset.UtcNow);

            memory.AppendToStream(
                "second",
                secondContentTokens,
                DateTimeOffset.UtcNow);

            Assert.Single(
                memory.Stream);

            Assert.Equal(
                "second",
                memory.Stream[0].Content);

            Assert.Equal(
                secondContentTokens +
                WorkingMemoryLimits.StreamEntryHeaderTokens,
                memory.StreamTokenCount);
        }

        [Fact]
        public void AppendToStream_Overflow_CanEvictMultipleOldEntries()
        {
            var memory =
                CreateWorkingMemory();

            memory.AppendToStream(
                "first",
                300,
                DateTimeOffset.UtcNow);

            memory.AppendToStream(
                "second",
                300,
                DateTimeOffset.UtcNow);

            memory.AppendToStream(
                "third",
                300,
                DateTimeOffset.UtcNow);

            memory.AppendToStream(
                "fourth",
                800,
                DateTimeOffset.UtcNow);

            Assert.Single(
                memory.Stream);

            Assert.Equal(
                "fourth",
                memory.Stream[0].Content);
        }

        [Fact]
        public void AppendToStream_RejectsEmptyContent()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Throws<
                ArgumentException>(
                    () =>
                        memory.AppendToStream(
                            string.Empty,
                            1,
                            DateTimeOffset.UtcNow));
        }

        [Fact]
        public void AppendToStream_RejectsNonPositiveTokenCount()
        {
            var memory =
                CreateWorkingMemory();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                    () =>
                        memory.AppendToStream(
                            "hello",
                            0,
                            DateTimeOffset.UtcNow));
        }

        [Fact]
        public void AppendToStream_RejectsSingleEntryLargerThanBudget()
        {
            var memory =
                CreateWorkingMemory();

            var contentTokens =
                WorkingMemoryLimits.MaxStreamTokens -
                WorkingMemoryLimits.StreamEntryHeaderTokens +
                1;

            Assert.Throws<
                ArgumentException>(
                    () =>
                        memory.AppendToStream(
                            "too large",
                            contentTokens,
                            DateTimeOffset.UtcNow));
        }

        [Fact]
        public void AppendToStream_AllowsEntryThatExactlyFillsBudget()
        {
            var memory =
                CreateWorkingMemory();

            var contentTokens =
                WorkingMemoryLimits.MaxStreamTokens -
                WorkingMemoryLimits.StreamEntryHeaderTokens;

            memory.AppendToStream(
                "exact fit",
                contentTokens,
                DateTimeOffset.UtcNow);

            Assert.Single(
                memory.Stream);

            Assert.Equal(
                WorkingMemoryLimits.MaxStreamTokens,
                memory.StreamTokenCount);
        }

        private static CitizenWorkingMemory
            CreateWorkingMemory()
        {
            return new CitizenWorkingMemory(
                citizenId:
                    "orestes");
        }
    }
}