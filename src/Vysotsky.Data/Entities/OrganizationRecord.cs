using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("organization")]
    public class OrganizationRecord : SortableEntity
    {
        [Column("name")] public string Name { get; init; } = null!;
    }
}
