using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("issue_comment_image")]
    public class IssueCommentImageRelation : Entity
    {
        [Column("issue_comment_id")] public long IssueCommentId { get; init; }
        [Column("image_id")] public long ImageId { get; init; }
    }
}