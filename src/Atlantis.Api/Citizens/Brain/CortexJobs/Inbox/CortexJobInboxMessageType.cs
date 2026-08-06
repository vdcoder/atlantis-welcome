namespace Atlantis.Api.Citizens.Brain.CortexJobs.Inbox;

public enum CortexJobInboxMessageType
{
    CortexTaskCompleted = 1,
    CortexTaskUnableToComplete = 2,
    PaymentFailed = 3
}