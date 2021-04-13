using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("issue")]
    public class IssueRecord : Entity
    {
        [Column("room_id")] public long RoomId { get; init; }
        [Column("area_id")] public long AreaId { get; init; }
        [Column("category_id")] public long? CategoryId { get; init; }

        [Column("status", DbType = "issue_status")]
        public IssueStatus Status { get; init; }

        [Column("title")] public string Title { get; init; } = null!;
        [Column("description")] public string Description { get; init; } = "";
        [Column("author_id")] public long AuthorId { get; init; }
        [Column("supervisor_id")] public long? SupervisorId { get; init; }
        [Column("worker_id")] public long? WorkerId { get; init; }
        [Column("note")] public string Note { get; init; } = "";
    }

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
