using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("issue_image")]
    public class IssueImageRelation : Entity
    {
        [Column("issue_id")] public long IssueId { get; init; }
        [Column("image_id")] public long ImageId { get; init; }
    }
}