using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("blocked_token")]
    public class BlockedTokenRecord : Entity
    {
        [Column("value")] public string Value { get; init; } = null!;
        [Column("user_id")] public long UserId { get; init; }
        [Column("issue_time")] public DateTimeOffset IssuedAt { get; init; }
        [Column("expiration_time")] public DateTimeOffset ExpirationTime { get; init; }
    }
}
