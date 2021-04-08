using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    public abstract class Entity
    {
        [Column("id")]
        [PrimaryKey]
        [Identity]
        public long Id { get; init; }
        [Column("created_at")]
        public DateTime CreatedAt { get; init; }
    }
}
