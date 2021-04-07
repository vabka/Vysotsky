using System;
using System.Threading.Tasks;

namespace Vysotsky.API.Controllers.Rooms
{
    internal interface IRoomRepository
    {
        Task<Room> CreateAsync(int roomPropertiesFloor, string roomPropertiesNumber, RoomStatus roomPropertiesStatus);
        Task<int> Count(DateTime untilDate);
        Task<Room[]> GetAll(DateTime untilDate, int limit, int offset);
    }
}
