using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("floor")]
    public class Floor : Entity
    {
        [Column("name")]
        public string? Name { get; init; }
        [Column("building_id")]
        public long BuildingId { get; init; }
    }
}
