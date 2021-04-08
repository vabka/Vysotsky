using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("organization")]
    public class Organization : Entity
    {
        [Column("name")] public string Name { get; init; } = null!;
        [Column("owner_id")] public long OwnerId { get; init; }
    }
}
