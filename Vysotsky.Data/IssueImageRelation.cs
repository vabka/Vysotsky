using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("issue_image")]
    public class IssueImageRelation
    {
        [Column("issue_id")] public long IssueId { get; init; }
        [Column("image_id")] public long ImageId { get; init; }
    }
}