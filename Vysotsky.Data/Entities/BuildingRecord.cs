
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("building")]
    public class BuildingRecord : Entity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
