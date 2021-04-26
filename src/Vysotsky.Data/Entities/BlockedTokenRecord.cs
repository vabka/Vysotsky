using System;
using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("blocked_token")]
    public class BlockedTokenRecord : Entity
    {
        [Column("jti")] public Guid Jti { get; init; }
        [Column("expiration_time")] public DateTimeOffset ExpirationTime { get; init; }
    }
}
