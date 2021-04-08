using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Table("room")]
    public class Room : Entity
    {
        [Column("name")] public string? Name { get; init; }
        [Column("number")] public string? Number { get; init; }
        [Column("owner_id")] public long? OwnerId { get; init; }
        [Column("floor_id")] public long FloorId { get; init; }
        [Column("status")] public RoomStatus Status { get; init; }
    }
}
