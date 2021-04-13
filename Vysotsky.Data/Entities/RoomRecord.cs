using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("room")]
    public class RoomRecord : Entity
    {
        [Column("name")] public string? Name { get; init; }
        [Column("number")] public string? Number { get; init; }
        [Column("owner_id")] public long? OwnerId { get; init; }
        [Column("floor_id")] public long FloorId { get; init; }

        [Column("status", DbType = "room_status")]
        public RoomStatus Status { get; init; }
    }

    public enum RoomStatus
    {
        [MapValue("Free")] Free,
        [MapValue("Owned")] Owned,
        [MapValue("Rented")] Rented,
        [MapValue("Unavailable")] Unavailable
    }
}
