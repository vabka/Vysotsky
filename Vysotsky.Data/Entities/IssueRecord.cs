using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("issue")]
    public class IssueRecord : VersionedEntity
    {
        [Column("room_id")] public long RoomId { get; init; }

        [Column("category_id")] public long CategoryId { get; init; }

        [Column("status", DbType = "issue_status")]
        public IssueStatus Status { get; init; }

        [Column("title")] public string Title { get; init; } = null!;

        [Column("description")] public string Description { get; init; } = "";
        [Column("author_has_unread_comments")] public bool HasUnreadComments { get; init; }

        [Column("author_id")] public long AuthorId { get; init; }

        [Column("supervisor_id")] public long? SupervisorId { get; init; }

        [Column("worker_id")] public long? WorkerId { get; init; }

        [Column("note")] public string Note { get; init; } = "";

        [Association(ThisKey = nameof(RoomId),
            OtherKey = nameof(RoomRecord.Id),
            CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public RoomRecord Room { get; init; } = null!;

        [Association(ThisKey = nameof(WorkerId),
            OtherKey = nameof(UserRecord.Id),
            CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public UserRecord? Worker { get; init; }

        [Association(ThisKey = nameof(SupervisorId),
            OtherKey = nameof(UserRecord.Id),
            CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public UserRecord? Supervisor { get; init; }

        [Association(ThisKey = nameof(AuthorId),
            OtherKey = nameof(UserRecord.Id),
            CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public UserRecord? Author { get; init; }

        [Association(ThisKey = nameof(CategoryId),
            OtherKey = nameof(CategoryRecord.Id),
            CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public CategoryRecord? Category { get; init; }
    }
}
