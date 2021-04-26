using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("category")]
    public class CategoryRecord : SortableEntity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
        [Column("image_id")]
        public long ImageId { get; init; }
        [Column("order")]
        public int? Order { get; init; }

    }
}
