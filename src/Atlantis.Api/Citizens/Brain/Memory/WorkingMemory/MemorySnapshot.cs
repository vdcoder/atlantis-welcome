namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public sealed class MemorySnapshot
    {
        private readonly string[] _lines;

        public MemorySnapshot(
            DateTimeOffset capturedAt,
            IReadOnlyList<string> lines)
        {
            if (lines.Count >
                WorkingMemoryLimits.MaxLines)
            {
                throw new ArgumentException(
                    $"Memory snapshot cannot contain more than " +
                    $"{WorkingMemoryLimits.MaxLines} lines.",
                    nameof(lines));
            }

            CapturedAt =
                capturedAt;

            _lines =
                lines.ToArray();
        }

        public DateTimeOffset CapturedAt
        {
            get;
        }

        public IReadOnlyList<string> Lines =>
            _lines;

        public string GetLine(
            int lineNumber)
        {
            if (lineNumber < 1 ||
                lineNumber >
                    WorkingMemoryLimits.MaxLines)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lineNumber));
            }

            var index =
                lineNumber - 1;

            if (index >= _lines.Length)
            {
                return string.Empty;
            }

            return _lines[index];
        }
    }
}