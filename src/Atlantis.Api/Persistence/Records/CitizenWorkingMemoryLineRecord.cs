namespace Atlantis.Api.Persistence.Records
{
    public sealed class CitizenWorkingMemoryLineRecord
    {
        public Guid Id
        {
            get;
            set;
        }

        public string CitizenId
        {
            get;
            set;
        } =
            string.Empty;

        public int LineNumber
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        } =
            string.Empty;

        public int ContentTokenCount
        {
            get;
            set;
        }

        public DateTimeOffset CreatedAt
        {
            get;
            set;
        }
    }
}
