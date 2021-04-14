using System;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities.Abstraction
{
    public abstract class VersionedEntity : SortableEntity
    {
        [Column("version")] [PrimaryKey] public long Version { get; init; }
        [Column("updated_at")] public DateTimeOffset UpdatedAt { get; init; }
    }
}