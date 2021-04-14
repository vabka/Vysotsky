
using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("building")]
    public class BuildingRecord : SortableEntity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
