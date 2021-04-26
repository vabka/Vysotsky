using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities.Abstraction
{
    public abstract class SortableEntity : EntityWithId
    {
        [Column("created_at")] public DateTimeOffset CreatedAt { get; init; }
    }
}
