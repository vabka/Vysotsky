using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("image")]
    public class ImageRecord : EntityWithId
    {
        [Column("external_id")] public string ExternalId { get; init; } = "";
    }
}