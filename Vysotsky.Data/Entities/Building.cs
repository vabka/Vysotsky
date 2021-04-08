
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("building")]
    public class Building : Entity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
