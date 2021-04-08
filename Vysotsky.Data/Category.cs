using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("category")]
    public class Category : Entity
    {
        [Column("area_id")]
        public long AreaId { get; init; }
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
