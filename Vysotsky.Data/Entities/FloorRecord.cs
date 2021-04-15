using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("floor")]
    public class FloorRecord : SortableEntity
    {
        [Column("number")] public string Number { get; init; } = null!;
        [Column("building_id")]
        public long BuildingId { get; init; }
    }
}
