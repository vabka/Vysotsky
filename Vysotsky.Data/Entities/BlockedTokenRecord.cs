using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("blocked_token")]
    public class BlockedTokenRecord
    {
        [Column("jti")] public Guid Jti { get; init; }
        [Column("expiration_time")] public DateTimeOffset ExpirationTime { get; init; }
    }
}
