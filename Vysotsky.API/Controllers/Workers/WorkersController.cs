using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers.Workers
{
    [Route(Resources.Workers)]
    public class WorkersController : ApiController
    {
        private readonly IUserService userService;

        public WorkersController(IUserService userService) => this.userService = userService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedUserDto>>>> GetAllWorkers()
        {
            var workers = await userService.GetAllUsersWithRoleAsync(UserRole.Worker);
            return Ok(workers.Select(w => w.ToDto()));
        }
    }
}
