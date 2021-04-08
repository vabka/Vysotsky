using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("token")]
    public class Token : Entity
    {
        [Column("value")] public string Value { get; init; } = null!;
    }
}
