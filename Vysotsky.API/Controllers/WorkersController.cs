using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Workers)]
    public class WorkersController : ApiController
    {
        private readonly IUserService _userService;

        public WorkersController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedData<PersistedUserDto>>>> GetAllWorkers(
            [FromQuery] PaginationParameters paginationParameters)
        {
            var (total, workers) = await _userService.GetAllUsersWithRoleAsync(UserRole.Worker,
                paginationParameters.Until, paginationParameters.ToTake(), paginationParameters.ToSkip());
            return Ok(PaginatedData.Create(paginationParameters, total, workers.Select(w => w.ToDto()).ToArray(),
                Resources.Workers));
        }
    }
}
