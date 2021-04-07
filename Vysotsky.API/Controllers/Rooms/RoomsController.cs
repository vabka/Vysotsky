using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Rooms
{
    /// <summary>
    /// Контроллер помещений
    /// </summary>
    [Route(Resources.Rooms)]
    public class RoomsController : ApiController
    {
        private readonly IRoomRepository _roomRepository;

        internal RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        /// <summary>
        /// Создать помещение
        /// </summary>
        /// <param name="roomProperties">Свойства комнаты</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Room>>> CreateRoom([FromBody] RoomProperties roomProperties)
        {
            //TODO room number is unique?
            //TODO floor range?
            //TODO room number format?
            return Ok(await _roomRepository.CreateAsync(roomProperties.Floor, roomProperties.Number,
                roomProperties.Status));
        }

        /// <summary>
        /// Получить список всех помещений
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedData<Room>>>> GetRooms(
            [FromQuery] PaginationParameters paginationParameters)
        {
            var start = paginationParameters.Until;
            var total = await _roomRepository.Count(start);
            var data = await _roomRepository.GetAll(start,
                paginationParameters.ToTake(), paginationParameters.ToSkip());
            return Ok(PaginatedData.Create(paginationParameters, total, data, Resources.Rooms));
        }
    }
}
