using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Buildings;
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
        private readonly IRoomService roomService;

        public SelfController(IUserService userService, ICurrentUserProvider currentUserProvider,
            IRoomService roomService)
        {
            this.userService = userService;
            this.currentUserProvider = currentUserProvider;
            this.roomService = roomService;
        }

        [HttpGet("organization/rooms")]
        public async Task<ActionResult<ApiResponse<WrappedListDto<PersistedRoomDto>>>>
            GetRoomsAttachedToUsersOrganization() =>
            currentUserProvider.CurrentUser.OrganizationId is not null and var orgId
                ? Ok((await roomService.GetRoomsByOrganizationIdAsync(orgId.Value)).Select(x => x.ToDto()).ToDto())
                : NotAuthorized("Current user is not attached to organization", "user.noOrganization");

        [HttpGet]
        public ActionResult<ApiResponse<PersistedUserDto>> Get() =>
            Ok(currentUserProvider.CurrentUser.ToDto());

        [HttpPut]
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
