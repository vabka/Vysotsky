using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class RoomService : IRoomService
    {
        private readonly VysotskyDataConnection _dataConnection;

        public RoomService(VysotskyDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public async Task<Building> CreateBuildingAsync(string name)
        {
            var id = await _dataConnection.Buildings
                .InsertWithInt64IdentityAsync(() => new BuildingRecord
                {
                    Name = name
                });
            return new Building
            {
                Id = id,
                Name = name
            };
        }

        public async Task<Building?> GetBuildingByIdOrNullAsync(long buildingId)
        {
            var result = await _dataConnection.Buildings
                .Select(b => new Building
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .SingleOrDefaultAsync(b => b.Id == buildingId);
            return result;
        }

        public async Task<Floor?> GetFloorByIdOrNullAsync(long floorId) =>
            await _dataConnection.Floors
                .Where(floor => floor.Id == floorId)
                .Select(floor => new Floor
                {
                    Id = floor.Id,
                    BuildingId = floor.BuildingId,
                    Number = floor.Number,
                })
                .SingleOrDefaultAsync();

        public Task<Building[]> GetAllBuildingsAsync() =>
            _dataConnection.Buildings
                .Select(b => new Building
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToArrayAsync();

        public Task<Floor[]> GetAllFloorsInBuildingAsync(Building building) =>
            _dataConnection.Floors
                .Where(f => f.BuildingId == building.Id)
                .Select(f => new Floor
                {
                    Id = f.Id,
                    Number = f.Number
                })
                .ToArrayAsync();

        public Task<Room[]> GetAllRoomsOnFloorAsync(Floor floor) =>
            _dataConnection.Rooms
                .Where(r => r.FloorId == floor.Id)
                .Select(r => new Room
                {
                    Id = r.Id,
                    Name = r.Name,
                    Number = r.Number,
                    Status = r.Status
                })
                .ToArrayAsync();

        public async Task<Floor> CreateFloor(Building building, string number)
        {
            var id = await _dataConnection.Floors.InsertWithInt64IdentityAsync(() => new FloorRecord
            {
                Number = number,
                BuildingId = building.Id,
            });
            return new Floor
            {
                Id = id,
                Number = number
            };
        }

        public async Task<Room> CreateRoom(Floor floor, string? name, string? number, RoomStatus status)
        {
            var id = await _dataConnection.Rooms.InsertWithInt64IdentityAsync(() => new RoomRecord
            {
                Number = number,
                Name = name,
                Status = status,
                FloorId = floor.Id,
            });
            return new Room
            {
                Id = id,
                Number = number,
                Name = name,
                Status = status
            };
        }
    }
}