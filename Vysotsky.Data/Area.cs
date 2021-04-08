using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("area")]
    public class Area : Entity
    {
        [Column("name")]
        public string Name { get; init; } = null!;
    }
}
