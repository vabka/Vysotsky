using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("area")]
    public class Area : Entity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
