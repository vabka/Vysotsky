using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("issue_comment")]
    public class IssueCommentRecord : SortableEntity
    {
        [Column("issue_id")] public long IssueId { get; init; }
        [Column("author_id")] public long AuthorId { get; init; }
        [Column("text")] public string Text { get; init; } = "";
    }
}
