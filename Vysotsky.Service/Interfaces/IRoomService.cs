using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<FullBuilding>> GetOrganizationBuildingsAsync(Organization organization);
        Task<Floor> CreateFloorAsync(Building building, string number);
        Task<Room> CreateRoomAsync(Floor floor, string? number, RoomStatus status, Organization? owner);
        Task<Building> CreateBuildingAsync(string name);
        Task<Building?> GetBuildingByIdOrNullAsync(long buildingId);
        Task<Floor?> GetFloorByIdOrNullAsync(long floorId);
        Task<Building[]> GetAllBuildingsAsync();
        Task<Floor[]> GetAllFloorsInBuildingAsync(Building building);
        Task<Room[]> GetAllRoomsOnFloorAsync(Floor floor);
        Task DeleteBuildingCascadeByIdAsync(long buildingId);
        Task<Room[]> GetRoomsAsync(long[] organizationRooms);
        Task<Room?> GetRoomByIdOrNullAsync(long roomId);
    }
}
