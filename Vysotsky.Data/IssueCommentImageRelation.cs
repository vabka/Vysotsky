using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("issue_comment_image")]
    public class IssueCommentImageRelation
    {
        [Column("issue_comment_id")] public long IssueCommentId { get; init; }
        [Column("image_id")] public long ImageId { get; init; }
    }
}