using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("image")]
    public class ImageRecord : Entity
    {
        [Column("external_id")] public string ExternalId { get; init; } = "";
    }
}