using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Tools;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class RoomService : IRoomService
    {
        private readonly VysotskyDataConnection _dataConnection;

        private static readonly Expression<Func<RoomRecord, Room>> MapToRoomExpr = x => new Room
        {
            Id = x.Id,
            Name = x.Name,
            Number = x.Number,
            Status = x.Status,
        };

        private static Room MapToRoom(RoomRecord x) =>
            new()
            {
                Id = x.Id,
                Name = x.Name,
                Number = x.Number,
                Status = x.Status,
            };

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

        public async Task<Building[]> GetAllBuildingsAsync() =>
            await _dataConnection.Buildings
                .OrderBy(b => b.CreatedAt)
                .Select(b => new Building
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToArrayAsync();

        public async Task<Floor[]> GetAllFloorsInBuildingAsync(Building building) =>
            await _dataConnection.Floors
                .OrderBy(b => b.CreatedAt)
                .Where(f => f.BuildingId == building.Id)
                .Select(f => new Floor
                {
                    Id = f.Id,
                    Number = f.Number
                })
                .ToArrayAsync();

        public async Task<Room[]> GetAllRoomsOnFloorAsync(Floor floor) =>
            await _dataConnection.Rooms
                .OrderBy(b => b.CreatedAt)
                .Where(r => r.FloorId == floor.Id)
                .Select(MapToRoomExpr)
                .ToArrayAsync();

        public async Task DeleteBuildingCascadeByIdAsync(long buildingId)
        {
            var buildingToDelete = _dataConnection.Buildings.Where(building => building.Id == buildingId);
            var allFloors = _dataConnection.Floors.Where(floor => floor.BuildingId == buildingId);
            var allRoomsInBuilding = from room in _dataConnection.Rooms
                                     join floor in _dataConnection.Floors on room.FloorId equals floor.Id
                                     where floor.BuildingId == buildingId
                                     select room;


            await using var transaction = await _dataConnection.BeginTransactionAsync();
            await allRoomsInBuilding.DeleteAsync();
            await allFloors.DeleteAsync();
            await buildingToDelete.DeleteAsync();
            await transaction.CommitAsync();
        }

        public async Task<Room[]> GetRoomsAsync(long[] organizationRooms) =>
            await _dataConnection.Rooms
                .Where(r => r.Id.In(organizationRooms))
                .Select(MapToRoomExpr)
                .ToArrayAsync();

        public async Task<Room?> GetRoomByIdOrNullAsync(long roomId) =>
            await _dataConnection.Rooms
                .Where(r => r.Id == roomId)
                .Select(MapToRoomExpr)
                .SingleOrDefaultAsync();

        public async Task<Floor> CreateFloorAsync(Building building, string number)
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

        public async Task<Room> CreateRoomAsync(Floor floor, string? name, string? number, RoomStatus status)
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

        public async Task<IEnumerable<FullBuilding>> GetOrganizationBuildingsAsync(Organization organization)
        {
            var roomsQuery = _dataConnection.Rooms
                .Where(x => x.OwnerId == organization.Id);

            var roomsData = await roomsQuery.ToArrayAsync();
            if (roomsData.Length == 0)
            {
                return Array.Empty<FullBuilding>();
            }

            var rooms = roomsData.GroupBy(x => x.FloorId, MapToRoom)
                .ToDictionary(x => x.Key, x => x);

            var floorsQuery = from room in roomsQuery
                              group room by room.FloorId
                into r
                              join floor in _dataConnection.Floors on r.Key equals floor.Id
                              select floor;
            var floorsData = await floorsQuery.ToArrayAsync();
            var floors = floorsData
                .GroupBy(x => x.BuildingId, x => new FullFloor
                {
                    Id = x.Id,
                    Number = x.Number,
                    Rooms = rooms[x.Id]
                }).ToDictionary(x => x.Key, x => x);

            var buildingsQuery = from floor in floorsQuery
                                 group floor by floor.BuildingId
                into f
                                 join building in _dataConnection.Buildings on f.Key equals building.Id
                                 select building;
            var buildingsData = await buildingsQuery.ToArrayAsync();
            return buildingsData.Select(x => new FullBuilding
            {
                Id = x.Id,
                Name = x.Name,
                Floors = floors[x.Id]
            })
                ;
        }
    }
}
