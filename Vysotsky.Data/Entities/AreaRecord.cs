using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("area")]
    public class AreaRecord : Entity
    {
        [Column("name")] public string Name { get; init; } = null!;
        [Column("image_id")] public long ImageId { get; init; }
    }
}