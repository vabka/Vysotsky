using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Buildings;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Buildings)]
    public class BuildingsController : ApiController
    {
        private readonly IRoomService _roomService;

        public BuildingsController(IRoomService roomService) => _roomService = roomService;

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ApiResponse<PersistedBuildingDto>>> CreateBuilding(
            [FromBody] BuildingDto buildingToCreate)
        {
            var building = await _roomService.CreateBuildingAsync(buildingToCreate.Name);
            return Created(Resources.Buildings.AppendPathSegment(building.Id), building.ToDto());
        }

        [HttpDelete("{buildingId:long}")]
        public async Task<ActionResult<ApiResponse>> DeleteBuilding(long buildingId)
        {
            await _roomService.DeleteBuildingCascadeByIdAsync(buildingId);
            return Ok();
        }

        [HttpPost("{buildingId:long}/floors")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ApiResponse<PersistedFloorDto>>> CreateFloor([FromRoute] long buildingId,
            [FromBody] FloorDto floorToCreate)
        {
            var building = await _roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await _roomService.CreateFloorAsync(building, floorToCreate.Number);
            return Created(Resources.Buildings.AppendPathSegments(buildingId, "floors", floor.Id),
                floor.ToDto()
            );
        }

        [HttpPost("{buildingId:long}/floors/{floorId:long}/rooms")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ApiResponse<PersistedRoomDto>>> CreateRoom([FromRoute] long buildingId,
            [FromRoute] long floorId,
            [FromBody] RoomDto roomToCreate)
        {
            var building = await _roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await _roomService.GetFloorByIdOrNullAsync(floorId);
            if (floor?.BuildingId != building.Id)
            {
                return FloorInBuildingNotFound(buildingId, floorId);
            }

            var room = await _roomService.CreateRoomAsync(floor, roomToCreate.Name, roomToCreate.Number,
                ToModel(roomToCreate.Status));
            return Created(
                Resources.Buildings.AppendPathSegments(buildingId, "floors", floorId, "rooms", room.Id),
                room.ToDto());
        }

        private NotFoundObjectResult FloorInBuildingNotFound(long buildingId, long floorId) =>
            NotFound($"Floor with id {floorId} not found in building with id {buildingId}",
                "buildings.floorInBuildingNotFound");

        private NotFoundObjectResult BuildingNotFound(long buildingId) => NotFound($"Building with id {buildingId} not found", "buildings.notFound");

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PersistedBuildingDto[]>>> GetAllBuildings()
        {
            var data = await _roomService.GetAllBuildingsAsync();
            return Ok(data.Select(b => b.ToDto()));
        }

        [HttpGet("{buildingId:long}/floors")]
        public async Task<ActionResult<ApiResponse<PersistedFloorDto[]>>> GetAllFloorsInBuilding(
            [FromRoute] long buildingId)
        {
            var building = await _roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var data = await _roomService.GetAllFloorsInBuildingAsync(building);
            return Ok(data.Select(f => f.ToDto()));
        }

        [HttpGet("{buildingId:long}/floors/{floorId:long}/rooms")]
        public async Task<ActionResult<ApiResponse<PersistedRoomDto[]>>> GetAllRoomsOnFloor([FromRoute] long buildingId,
            [FromRoute] long floorId)
        {
            var building = await _roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await _roomService.GetFloorByIdOrNullAsync(floorId);
            if (floor?.BuildingId != building.Id)
            {
                return FloorInBuildingNotFound(buildingId, floorId);
            }

            var rooms = await _roomService.GetAllRoomsOnFloorAsync(floor);
            return Ok(rooms
                .Select(r => r.ToDto())
            );
        }

        private static RoomStatus ToModel(RoomStatusDto statusDto) =>
            statusDto switch
            {
                RoomStatusDto.Free => RoomStatus.Free,
                RoomStatusDto.Owned => RoomStatus.Owned,
                RoomStatusDto.Rented => RoomStatus.Rented,
                RoomStatusDto.Unavalable => RoomStatus.Unavailable,
                _ => throw new InvalidOperationException()
            };
    }
}
