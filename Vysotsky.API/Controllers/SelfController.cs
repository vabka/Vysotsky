using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Self)]
    public class SelfController : ApiController
    {
        private readonly IUserService userService;
        private readonly ICurrentUserProvider currentUserProvider;

        public SelfController(IUserService userService, ICurrentUserProvider currentUserProvider)
        {
            this.userService = userService;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> Get() =>
            Ok(await userService.GetUserByIdOrNull(currentUserProvider.CurrentUser.Id));

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Post([FromBody] EditableUserFieldsDto user)
        {
            await userService.UpdateUserAsync(currentUserProvider.CurrentUser,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Contacts.Select(u => u.ToModel()));
            return Ok();
        }
    }
}
