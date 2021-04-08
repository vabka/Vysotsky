using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Rooms
{
    /// <summary>
    /// Контроллер помещений
    /// </summary>
    [Route(Resources.Rooms)]
    public class RoomsController : ApiController
    {
        /// <summary>
        /// Создать помещение
        /// </summary>
        /// <param name="roomDto">Свойства комнаты</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult<ApiResponse<Room>>> CreateRoom([FromBody] RoomDto roomDto)
        {
            //TODO room number is unique?
            //TODO floor range?
            //TODO room number format?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить список всех помещений
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult<ApiResponse<PaginatedData<Room>>>> GetRooms(
            [FromQuery] PaginationParameters paginationParameters)
        {
            throw new NotImplementedException();
        }
    }
}
