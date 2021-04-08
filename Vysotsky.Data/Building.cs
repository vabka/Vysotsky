
using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("building")]
    public class Building : Entity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
