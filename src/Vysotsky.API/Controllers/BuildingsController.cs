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
        private readonly IRoomService roomService;

        public BuildingsController(IRoomService roomService) => this.roomService = roomService;

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ApiResponse<PersistedBuildingDto>>> CreateBuilding(
            [FromBody] BuildingDto buildingToCreate)
        {
            var building = await roomService.CreateBuildingAsync(buildingToCreate.Name);
            return Created(Resources.Buildings.AppendPathSegment(building.Id), building.ToDto());
        }

        [HttpDelete("{buildingId:long}")]
        public async Task<ActionResult<ApiResponse>> DeleteBuilding(long buildingId)
        {
            await roomService.DeleteBuildingCascadeByIdAsync(buildingId);
            return Ok();
        }

        [HttpPost("{buildingId:long}/floors")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ApiResponse<PersistedFloorDto>>> CreateFloor([FromRoute] long buildingId,
            [FromBody] FloorDto floorToCreate)
        {
            var building = await roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await roomService.CreateFloorAsync(building, floorToCreate.Number);
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
            var building = await roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await roomService.GetFloorByIdOrNullAsync(floorId);
            if (floor?.BuildingId != building.Id)
            {
                return FloorInBuildingNotFound(buildingId, floorId);
            }

            //TODO !!!
            var room = await roomService.CreateRoomAsync(floor, roomToCreate.Number,
                ToModel(roomToCreate.Status), null);
            return Created(
                Resources.Buildings.AppendPathSegments(buildingId, "floors", floorId, "rooms", room.Id),
                room.ToDto());
        }

        private NotFoundObjectResult FloorInBuildingNotFound(long buildingId, long floorId) =>
            NotFound($"Floor with id {floorId} not found in building with id {buildingId}",
                "buildings.floorInBuildingNotFound");

        private NotFoundObjectResult BuildingNotFound(long buildingId) =>
            NotFound($"Building with id {buildingId} not found", "buildings.notFound");

        [HttpGet]
        public async Task<ActionResult<ApiResponse<WrappedListDto<PersistedBuildingDto>>>> GetAllBuildings()
        {
            var data = await roomService.GetAllBuildingsAsync();
            return Ok(data.Select(b => b.ToDto()).ToDto());
        }

        [HttpGet("{buildingId:long}/floors")]
        public async Task<ActionResult<ApiResponse<WrappedListDto<PersistedFloorDto>>>> GetAllFloorsInBuilding(
            [FromRoute] long buildingId)
        {
            var building = await roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var data = await roomService.GetAllFloorsInBuildingAsync(building);
            return Ok(data.Select(f => f.ToDto()).ToDto());
        }

        [HttpGet("{buildingId:long}/floors/{floorId:long}/rooms")]
        public async Task<ActionResult<ApiResponse<WrappedListDto<PersistedRoomDto>>>> GetAllRoomsOnFloor(
            [FromRoute] long buildingId,
            [FromRoute] long floorId)
        {
            var building = await roomService.GetBuildingByIdOrNullAsync(buildingId);
            if (building == null)
            {
                return BuildingNotFound(buildingId);
            }

            var floor = await roomService.GetFloorByIdOrNullAsync(floorId);
            if (floor?.BuildingId != building.Id)
            {
                return FloorInBuildingNotFound(buildingId, floorId);
            }

            var rooms = await roomService.GetAllRoomsOnFloorAsync(floor);
            return Ok(rooms
                .Select(r => r.ToDto())
                .ToDto()
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
