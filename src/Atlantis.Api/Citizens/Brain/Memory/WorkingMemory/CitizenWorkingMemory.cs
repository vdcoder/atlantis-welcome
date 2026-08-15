namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed class CitizenWorkingMemory
    {
        public CitizenWorkingMemory(
            string citizenId)
        {
            CitizenId =
                citizenId;
        }

        public CitizenWorkingMemory(
            string citizenId,
            IReadOnlyList<WorkingMemoryLine> lines,
            IReadOnlyList<MemoryStreamEntry> stream)
        {
            CitizenId =
                citizenId;

            foreach (var line in lines)
            {
                if (line.LineNumber < 1 ||
                    line.LineNumber >
                        WorkingMemoryLimits.MaxLines)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(lines));
                }

                _lines[
                    line.LineNumber - 1] =
                        line;
            }

            foreach (var entry in stream)
            {
                _stream.Add(
                    entry);

                _streamTokenCount +=
                    entry.BudgetTokenCount;
            }

            if (_streamTokenCount >
                WorkingMemoryLimits.MaxStreamTokens)
            {
                throw new ArgumentException(
                    "Loaded memory stream exceeds its token budget.",
                    nameof(stream));
            }
        }

        public string CitizenId
        {
            get;
        }

        private readonly WorkingMemoryLine?[]
            _lines =
                new WorkingMemoryLine?[
                    WorkingMemoryLimits.MaxLines];

        private readonly List<MemoryStreamEntry>
            _stream =
                new List<MemoryStreamEntry>();

        private int _streamTokenCount;

        public IReadOnlyList<WorkingMemoryLine?>
            Lines =>
                _lines;

        public IReadOnlyList<MemoryStreamEntry>
            Stream =>
                _stream;

        public int StreamTokenCount =>
            _streamTokenCount;

        public string GetLineContent(
            int lineNumber)
        {
            if (lineNumber < 1 ||
                lineNumber >
                    WorkingMemoryLimits.MaxLines)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lineNumber));
            }

            return _lines[
                lineNumber - 1]
                ?.Content ??
                string.Empty;
        }

        public void UpdateLine(
            int lineNumber,
            string content)
        {
            if (lineNumber < 1 ||
                lineNumber >
                    WorkingMemoryLimits.MaxLines)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lineNumber));
            }

            // validate token budget
            // persist append-only line
            // replace _lines[lineNumber - 1]
        }

        public void AppendToStream(
            string content,
            int tokenCount,
            DateTimeOffset createdAt)
        {
            if (string.IsNullOrWhiteSpace(
                    content))
            {
                throw new ArgumentException(
                    "Memory stream entry cannot be empty.",
                    nameof(content));
            }

            if (tokenCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tokenCount));
            }

            var entry =
                new MemoryStreamEntry(
                    Guid.NewGuid(),
                    content,
                    tokenCount,
                    createdAt);

            if (entry.BudgetTokenCount >
                WorkingMemoryLimits.MaxStreamTokens)
            {
                throw new ArgumentException(
                    $"A single stream entry cannot exceed " +
                    $"{WorkingMemoryLimits.MaxStreamTokens} tokens " +
                    $"including entry overhead.",
                    nameof(tokenCount));
            }

            _stream.Add(
                entry);

            _streamTokenCount +=
                entry.BudgetTokenCount;

            while (_streamTokenCount >
                   WorkingMemoryLimits.MaxStreamTokens)
            {
                var oldest =
                    _stream[0];

                _stream.RemoveAt(
                    0);

                _streamTokenCount -=
                    oldest.BudgetTokenCount;
            }
        }
    }
}