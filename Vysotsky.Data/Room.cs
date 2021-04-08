using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Table("room")]
    public class Room : Entity
    {
        [Column("owner_id")]
        public long? OwnerId { get; init; }
        [Column("floor_id")]
        public long FloorId { get; init; }
        [Column("status")]
        public RoomStatus Status { get; init; }
    }
}
