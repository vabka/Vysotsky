using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("issue_comment")]
    public class IssueComment : Entity
    {
        [Column("issue_id")] public long IssueId { get; init; }
        [Column("author_id")] public long AuthorId { get; init; }
        [Column("text")] public string Text { get; init; } = "";
    }
}
