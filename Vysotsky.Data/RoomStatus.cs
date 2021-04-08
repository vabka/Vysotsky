using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    public enum RoomStatus
    {
        [MapValue("Free")] Free,
        [MapValue("Owned")] Owned,
        [MapValue("Rented")] Rented,
        [MapValue("Unavailable")] Unavailable
    }
}
