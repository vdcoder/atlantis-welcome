using Atlantis.Api.Citizens.Brain.Memory.WorkingMemory;
using Xunit;

namespace Atlantis.Api.IntegrationTests.Memory
{
    public sealed class CitizenWorkingMemoryTests
    {
        [Fact]
        public void Snapshot_ReturnsStoredLine()
        {
            var snapshot =
                CreateSnapshot(
                    "alpha",
                    "beta");

            Assert.Equal(
                "alpha",
                snapshot.GetLine(1));

            Assert.Equal(
                "beta",
                snapshot.GetLine(2));
        }

        [Fact]
        public void Snapshot_ReturnsEmptyForValidUnpopulatedLine()
        {
            var snapshot =
                CreateSnapshot(
                    "alpha");

            Assert.Equal(
                string.Empty,
                snapshot.GetLine(2));

            Assert.Equal(
                string.Empty,
                snapshot.GetLine(
                    WorkingMemoryLimits.MaxLines));
        }

        [Fact]
        public void Snapshot_RejectsLineBelowRange()
        {
            var snapshot =
                CreateSnapshot();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                    () =>
                        snapshot.GetLine(0));
        }

        [Fact]
        public void Snapshot_RejectsLineAboveRange()
        {
            var snapshot =
                CreateSnapshot();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                    () =>
                        snapshot.GetLine(
                            WorkingMemoryLimits.MaxLines +
                            1));
        }

        [Fact]
        public void Snapshot_RejectsTooManyLines()
        {
            var lines =
                Enumerable
                    .Range(
                        1,
                        WorkingMemoryLimits.MaxLines + 1)
                    .Select(
                        number =>
                            $"line-{number}")
                    .ToArray();

            Assert.Throws<
                ArgumentException>(
                    () =>
                        new MemorySnapshot(
                            DateTimeOffset.UtcNow,
                            lines));
        }

        [Fact]
        public void UpdateLine_OverridesSnapshotValue()
        {
            var memory =
                CreateWorkingMemory(
                    "original");

            memory.UpdateLine(
                1,
                "changed");

            Assert.Equal(
                "changed",
                memory.GetEffectiveLine(1));

            Assert.Equal(
                "changed",
                memory.Overrides[1]);
        }

        [Fact]
        public void UpdateLine_WhenMatchingSnapshot_RemovesOverride()
        {
            var memory =
                CreateWorkingMemory(
                    "original");

            memory.UpdateLine(
                1,
                "changed");

            memory.UpdateLine(
                1,
                "original");

            Assert.Equal(
                "original",
                memory.GetEffectiveLine(1));

            Assert.False(
                memory.Overrides.ContainsKey(1));
        }

        [Fact]
        public void UpdateLine_EmptyContent_ClearsSnapshotValue()
        {
            var memory =
                CreateWorkingMemory(
                    "remember this");

            memory.UpdateLine(
                1,
                string.Empty);

            Assert.Equal(
                string.Empty,
                memory.GetEffectiveLine(1));

            Assert.True(
                memory.Overrides.ContainsKey(1));

            Assert.Equal(
                string.Empty,
                memory.Overrides[1]);
        }

        [Fact]
        public void CaptureSnapshot_MaterializesEffectiveMemory()
        {
            var memory =
                CreateWorkingMemory(
                    "one",
                    "two",
                    "three");

            memory.UpdateLine(
                2,
                "changed two");

            memory.CaptureSnapshot(
                DateTimeOffset.UtcNow,
                50);

            Assert.Equal(
                "one",
                memory.Snapshot.GetLine(1));

            Assert.Equal(
                "changed two",
                memory.Snapshot.GetLine(2));

            Assert.Equal(
                "three",
                memory.Snapshot.GetLine(3));
        }

        [Fact]
        public void CaptureSnapshot_PreservesExplicitlyClearedLine()
        {
            var memory =
                CreateWorkingMemory(
                    "one");

            memory.UpdateLine(
                1,
                string.Empty);

            memory.CaptureSnapshot(
                DateTimeOffset.UtcNow,
                50);

            Assert.Equal(
                string.Empty,
                memory.Snapshot.GetLine(1));
        }

        [Fact]
        public void CaptureSnapshot_ClearsOverrides()
        {
            var memory =
                CreateWorkingMemory(
                    "one");

            memory.UpdateLine(
                1,
                "changed");

            memory.CaptureSnapshot(
                DateTimeOffset.UtcNow,
                50);

            Assert.Empty(
                memory.Overrides);
        }

        [Fact]
        public void CaptureSnapshot_UpdatesTimestampAndCountdown()
        {
            var memory =
                CreateWorkingMemory(
                    "one");

            var capturedAt =
                DateTimeOffset.UtcNow;

            memory.CaptureSnapshot(
                capturedAt,
                77);

            Assert.Equal(
                capturedAt,
                memory.Snapshot.CapturedAt);

            Assert.Equal(
                77,
                memory.CaptureCountdown);
        }

        [Fact]
        public void EasyRemember_AppendsEntriesInOrder()
        {
            var memory =
                CreateWorkingMemory();

            var firstAt =
                DateTimeOffset.UtcNow;

            var secondAt =
                firstAt.AddSeconds(1);

            memory.EasyRemember(
                "first",
                10,
                firstAt);

            memory.EasyRemember(
                "second",
                20,
                secondAt);

            Assert.Equal(
                2,
                memory.EasyLog.Count);

            Assert.Equal(
                "first",
                memory.EasyLog[0].Content);

            Assert.Equal(
                "second",
                memory.EasyLog[1].Content);
        }

        [Fact]
        public void EasyRemember_TokenCountIncludesEntryHeader()
        {
            var memory =
                CreateWorkingMemory();

            memory.EasyRemember(
                "hello",
                10,
                DateTimeOffset.UtcNow);

            Assert.Equal(
                10 +
                WorkingMemoryLimits.EasyLogEntryHeaderTokens,
                memory.EasyLogTokenCount);
        }

        [Fact]
        public void EasyRemember_UnderBudget_DoesNotEvict()
        {
            var memory =
                CreateWorkingMemory();

            memory.EasyRemember(
                "first",
                100,
                DateTimeOffset.UtcNow);

            memory.EasyRemember(
                "second",
                100,
                DateTimeOffset.UtcNow);

            Assert.Equal(
                2,
                memory.EasyLog.Count);
        }

        [Fact]
        public void EasyRemember_Overflow_EvictsOldestWholeEntry()
        {
            var memory =
                CreateWorkingMemory();

            var firstContentTokens =
                600;

            var secondContentTokens =
                500;

            memory.EasyRemember(
                "first",
                firstContentTokens,
                DateTimeOffset.UtcNow);

            memory.EasyRemember(
                "second",
                secondContentTokens,
                DateTimeOffset.UtcNow);

            Assert.Single(
                memory.EasyLog);

            Assert.Equal(
                "second",
                memory.EasyLog[0].Content);

            Assert.Equal(
                secondContentTokens +
                WorkingMemoryLimits.EasyLogEntryHeaderTokens,
                memory.EasyLogTokenCount);
        }

        [Fact]
        public void EasyRemember_Overflow_CanEvictMultipleOldEntries()
        {
            var memory =
                CreateWorkingMemory();

            memory.EasyRemember(
                "first",
                300,
                DateTimeOffset.UtcNow);

            memory.EasyRemember(
                "second",
                300,
                DateTimeOffset.UtcNow);

            memory.EasyRemember(
                "third",
                300,
                DateTimeOffset.UtcNow);

            memory.EasyRemember(
                "fourth",
                800,
                DateTimeOffset.UtcNow);

            Assert.Single(
                memory.EasyLog);

            Assert.Equal(
                "fourth",
                memory.EasyLog[0].Content);
        }

        [Fact]
        public void EasyRemember_RejectsSingleEntryLargerThanBudget()
        {
            var memory =
                CreateWorkingMemory();

            var contentTokens =
                WorkingMemoryLimits.MaxEasyLogTokens -
                WorkingMemoryLimits.EasyLogEntryHeaderTokens +
                1;

            Assert.Throws<
                ArgumentException>(
                    () =>
                        memory.EasyRemember(
                            "too large",
                            contentTokens,
                            DateTimeOffset.UtcNow));
        }

        private static CitizenWorkingMemory
            CreateWorkingMemory(
                params string[] lines)
        {
            return new CitizenWorkingMemory(
                citizenId:
                    "orestes",

                snapshot:
                    CreateSnapshot(
                        lines),

                captureCountdown:
                    100);
        }

        private static MemorySnapshot
            CreateSnapshot(
                params string[] lines)
        {
            return new MemorySnapshot(
                capturedAt:
                    DateTimeOffset.UtcNow,

                lines:
                    lines);
        }
    }
}