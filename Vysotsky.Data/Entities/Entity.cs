using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    public abstract class Entity
    {
        [Column("id")]
        [PrimaryKey]
        [Identity]
        public long Id { get; init; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; init; }
    }
}
