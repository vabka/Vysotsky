using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("token")]
    public class Token : Entity
    {
        [Column("value")] public string Value { get; init; } = null!;
        [Column("user_id")] public long UserId { get; init; }
    }
}
