using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("floor")]
    public class Floor : Entity
    {
        [Column("number")] public string Number { get; init; } = null!;
        [Column("building_id")]
        public long BuildingId { get; init; }
    }
}
