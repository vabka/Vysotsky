using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("category")]
    public class CategoryRecord : SortableEntity
    {
        [Column("area_id")]
        public long AreaId { get; init; }
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
