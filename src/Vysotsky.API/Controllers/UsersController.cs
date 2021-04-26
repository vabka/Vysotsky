using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Users)]
    public class UsersController : ApiController
    {
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        private readonly IAtomicService atomicService;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IRoomService roomService;

        public UsersController(IOrganizationService organizationService,
            IUserService userService,
            IAtomicService atomicService,
            ICurrentUserProvider currentUserProvider,
            IRoomService roomService)
        {
            this.organizationService = organizationService;
            this.userService = userService;
            this.atomicService = atomicService;
            this.currentUserProvider = currentUserProvider;
            this.roomService = roomService;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> GetUser(string username)
        {
            var user = await userService.GetUserByUsernameOrNullAsync(username);
            return user switch
            {
                null => UserNotFound(username),
                { OrganizationId: not null and var userOrganizationId }
                    when !currentUserProvider.CurrentUser.CanReadOrganization(userOrganizationId.Value) =>
                    NotAuthorized("Customer cant access another customer", "users.notAuthorized"),
                _ => Ok(user.ToDto())
            };
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> RegisterUser(UserDto user)
        {
            var alreadyCreatedUser = await userService.GetUserByUsernameOrNullAsync(user.Username);
            if (alreadyCreatedUser != null)
            {
                return BadRequest("User with same username exists", "users.usernameExists");
            }

            if (user.Role == UserRoleDto.CustomerRepresentative)
            {
                return BadRequest("Representative can be created only in organization",
                    "users.cannotCreateUnattachedRepresentative");
            }

            await using var transaction = await atomicService.BeginAtomicOperationAsync();
            Organization? organization = null;
            if (user.Role == UserRoleDto.Customer)
            {
                if (user.Organization == null)
                {
                    return BadRequest("Organization field in necessary", "users.necessaryFieldMissing");
                }

                var rooms = await roomService.GetRoomsAsync(user.Organization.Rooms);
                if (rooms.Any(room => room.OwnerId.HasValue))
                {
                    return BadRequest("Can't obtain room", "rooms.occupied");
                }

                if (rooms.Length != user.Organization.Rooms.Length)
                {
                    return BadRequest("Some rooms are not exist", "rooms.notFound");
                }

                organization = await organizationService.CreateOrganizationAsync(user.Organization.Name, rooms);
            }

            var createdUser = await userService.CreateUserAsync(user.Username,
                user.Password,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Contacts
                    .Select(c => new UserContact
                    {
                        Name = c.Name,
                        Value = c.Value,
                        Type = ToModel(c.Type)
                    }),
                ToModel(user.Role),
                organization);
            await transaction.CompleteAsync();
            return Created(Resources.Users.AppendPathSegment(createdUser.Username), createdUser.ToDto());
        }

        private static UserContactType ToModel(UserContactTypeDto c) =>
            c switch
            {
                UserContactTypeDto.Phone => UserContactType.Phone,
                _ => throw new InvalidOperationException()
            };

        private static UserRole ToModel(UserRoleDto role) =>
            role switch
            {
                UserRoleDto.Supervisor => UserRole.Supervisor,
                UserRoleDto.Worker => UserRole.Worker,
                UserRoleDto.Customer => UserRole.OrganizationOwner,
                UserRoleDto.CustomerRepresentative => UserRole.OrganizationMember,
                _ => throw new InvalidOperationException()
            };

        private NotFoundObjectResult UserNotFound(string username) => NotFound($"User by not found by id {username}", "users.userNotFound");
    }
}
