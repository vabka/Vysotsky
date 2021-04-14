using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("room")]
    public class RoomRecord : SortableEntity
    {
        [Column("name")] public string? Name { get; init; }
        [Column("number")] public string? Number { get; init; }
        [Column("owner_id")] public long? OwnerId { get; init; }
        [Column("floor_id")] public long FloorId { get; init; }

        [Column("status", DbType = "room_status")]
        public RoomStatus Status { get; init; }
    }
}
