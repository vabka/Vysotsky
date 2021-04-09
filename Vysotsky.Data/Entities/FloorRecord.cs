using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("floor")]
    public class FloorRecord : Entity
    {
        [Column("number")] public string Number { get; init; } = null!;
        [Column("building_id")]
        public long BuildingId { get; init; }
    }
}
