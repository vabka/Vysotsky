using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("area")]
    public class AreaRecord : SortableEntity
    {
        [Column("name")] public string Name { get; init; } = null!;
        [Column("image_id")] public long ImageId { get; init; }
    }
}