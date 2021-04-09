using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("organization")]
    public class OrganizationRecord : Entity
    {
        [Column("name")] public string Name { get; init; } = null!;
        [Column("owner_id")] public long OwnerId { get; init; }
    }
}
