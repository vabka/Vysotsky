using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("issue")]
    public class Issue : Entity
    {
        [Column("room_id")] public long RoomId { get; init; }
        [Column("area_id")] public long AreaId { get; init; }
        [Column("category_id")] public long? CategoryId { get; init; }
        [Column("status")] public IssueStatus Status { get; init; }
        [Column("title")] public string Title { get; init; } = null!;
        [Column("description")] public string Description { get; init; } = "";
        [Column("author_id")] public long AuthorId { get; init; }
        [Column("supervisor_id")] public long? SupervisorId { get; init; }
        [Column("worker_id")] public long? WorkerId { get; init; }
        [Column("note")] public string Note { get; init; } = "";
    }
}
