namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed class CitizenWorkingMemory
    {
        public CitizenWorkingMemory(
            string citizenId,
            MemorySnapshot snapshot,
            int captureCountdown)
        {
            CitizenId =
                citizenId;

            Snapshot =
                snapshot;

            CaptureCountdown =
                captureCountdown;
        }

        public string CitizenId
        {
            get;
        }

        public MemorySnapshot Snapshot
        {
            get;
            private set;
        }

        public int CaptureCountdown
        {
            get;
            private set;
        }

        private readonly Dictionary<int, string>
            _overrides =
                new Dictionary<int, string>();

        private readonly List<EasyLogEntry>
            _easyLog =
                new List<EasyLogEntry>();

        private int _easyLogTokenCount;

        public IReadOnlyDictionary<int, string>
            Overrides =>
                _overrides;

        public IReadOnlyList<EasyLogEntry>
            EasyLog =>
                _easyLog;

        public int EasyLogTokenCount =>
            _easyLogTokenCount;

        public string GetEffectiveLine(
            int lineNumber)
        {
            if (_overrides.TryGetValue(
                    lineNumber,
                    out var overriddenValue))
            {
                return overriddenValue;
            }

            return Snapshot.GetLine(
                lineNumber);
        }

        public void UpdateLine(
            int lineNumber,
            string content)
        {
            var snapshotValue =
                Snapshot.GetLine(
                    lineNumber);

            if (content == snapshotValue)
            {
                _overrides.Remove(
                    lineNumber);

                return;
            }

            _overrides[lineNumber] =
                content;
        }

        public void CaptureSnapshot(
            DateTimeOffset capturedAt,
            int nextCaptureCountdown)
        {
            var lines =
                new string[
                    WorkingMemoryLimits.MaxLines];

            for (var lineNumber = 1;
                 lineNumber <=
                     WorkingMemoryLimits.MaxLines;
                 lineNumber++)
            {
                lines[lineNumber - 1] =
                    GetEffectiveLine(
                        lineNumber);
            }

            Snapshot =
                new MemorySnapshot(
                    capturedAt,
                    lines);

            _overrides.Clear();

            CaptureCountdown =
                nextCaptureCountdown;
        }

        public void EasyRemember(
            string content,
            int tokenCount,
            DateTimeOffset createdAt)
        {
            if (string.IsNullOrWhiteSpace(
                    content))
            {
                throw new ArgumentException(
                    "Easy log memory cannot be empty.",
                    nameof(content));
            }

            if (tokenCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tokenCount));
            }

            var entry =
                new EasyLogEntry(
                    Guid.NewGuid(),
                    content,
                    tokenCount,
                    createdAt);

            if (entry.BudgetTokenCount >
                WorkingMemoryLimits.MaxEasyLogTokens)
            {
                throw new ArgumentException(
                    $"A single easy log entry cannot exceed " +
                    $"{WorkingMemoryLimits.MaxEasyLogTokens} tokens " +
                    $"including entry overhead.",
                    nameof(tokenCount));
            }

            _easyLog.Add(
                entry);

            _easyLogTokenCount +=
                entry.BudgetTokenCount;

            while (_easyLogTokenCount >
                   WorkingMemoryLimits.MaxEasyLogTokens)
            {
                var oldest =
                    _easyLog[0];

                _easyLog.RemoveAt(
                    0);

                _easyLogTokenCount -=
                    oldest.BudgetTokenCount;
            }
        }
    }
}