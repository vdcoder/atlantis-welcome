namespace Atlantis.Api.Citizens.Brain.Memory.WorkingMemory
{
    public static class WorkingMemoryLimits
    {
        public const int StaticLineCount =
            20;

        public const int DynamicLineCount =
            20;

        public const int MaxLines =
            StaticLineCount +
            DynamicLineCount;

        public const int MaxLineTokens =
            128;

        public const int MaxStreamTokens =
            1024;

        public const int StreamEntryHeaderTokens =
            4;
    }
}
