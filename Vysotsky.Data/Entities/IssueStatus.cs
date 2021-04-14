using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    public enum IssueStatus
    {
        [MapValue("New")] New,
        [MapValue("Cancelled")] CancelledByCustomer,
        [MapValue("NeedInfo")] NeedInfo,
        [MapValue("Rejected")] Rejected,
        [MapValue("InProgress")] InProgress,
        [MapValue("Completed")] Completed,
        [MapValue("Accepted")] Accepted,
        [MapValue("Closed")] Closed
    }
}