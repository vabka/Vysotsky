using System.Threading.Tasks;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IRoomService
    {
        Task<Floor> CreateFloor(Building building, string number);
        Task<Room> CreateRoom(Floor floor, string? name, string? number, RoomStatus status);
        Task<Building> CreateBuildingAsync(string name);
        Task<Building?> GetBuildingByIdOrNullAsync(long buildingId);
        Task<Floor?> GetFloorByIdOrNullAsync(long floorId);
        Task<Building[]> GetAllBuildingsAsync();
        Task<Floor[]> GetAllFloorsInBuildingAsync(Building building);
        Task<Room[]> GetAllRoomsOnFloorAsync(Floor floor);
    }
}