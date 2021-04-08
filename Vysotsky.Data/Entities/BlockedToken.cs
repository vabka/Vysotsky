using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("token_block_list")]
    public class BlockedToken : Entity
    {
        [Column("value")] public string Value { get; init; } = null!;
        [Column("user_id")] public long UserId { get; init; }
        [Column("issue_time")] public DateTimeOffset IssuedAt { get; init; }
        [Column("expiration_time")] public DateTimeOffset ExpirationTime { get; init; }
    }
}
