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
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public SelfController(IUserService userService, ICurrentUserProvider currentUserProvider)
        {
            _userService = userService;
            _currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> Get() =>
            Ok(await _userService.GetUserByIdOrNull(_currentUserProvider.CurrentUser.Id));

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Post([FromBody] EditableUserFieldsDto user)
        {
            await _userService.UpdateUserAsync(_currentUserProvider.CurrentUser,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Contacts.Select(u => u.ToModel()));
            return Ok();
        }
    }
}
