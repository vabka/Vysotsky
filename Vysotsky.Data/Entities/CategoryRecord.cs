using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("category")]
    public class CategoryRecord : Entity
    {
        [Column("area_id")]
        public long AreaId { get; init; }
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
