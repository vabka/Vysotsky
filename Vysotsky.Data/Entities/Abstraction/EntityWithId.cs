using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities.Abstraction
{
    public abstract class EntityWithId : Entity
    {
        [Column("id")] [PrimaryKey] [Identity] public long Id { get; init; }
    }
}