using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("issue")]
    public class Issue : Entity
    {
        [Column("area_id")] public long AreaId { get; init; }
        [Column("category_id")] public long? CategoryId { get; init; }
        [Column("status")] public IssueStatus Status { get; init; }
        [Column("title")] public string Title { get; init; } = null!;
        [Column("description")] public string Description { get; init; } = "";
        [Column("creator_id")] public long CreatorId { get; init; }
        [Column("supervisor_id")] public long? SupervisorId { get; init; }
        [Column("worker_id")] public long? WorkerId { get; init; }
        [Column("note")] public string Note { get; init; } = "";
    }
}
