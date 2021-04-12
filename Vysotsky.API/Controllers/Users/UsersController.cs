using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Controllers.Organizations.Dto;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers.Users
{
    [Route(Resources.Users)]
    public class UsersController : ApiController
    {
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;

        public UsersController(IOrganizationService organizationService, IUserService userService)
        {
            _organizationService = organizationService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> RegisterUser(UserDto user)
        {
            var usr = await _userService.RegisterUserAsync(user.Credentials.Username,
                user.Credentials.Password,
                user.Name.FirstName,
                user.Name.LastName,
                user.Name.Patronymic,
                user.Contacts
                    .Select(c => new Data.Entities.UserContact
                    {
                    })
                    .ToArray(),
                ToModel(user));
            return Created(Resources.Organizations.AppendPathSegment(usr.Username), new PersistedUserDto
            {
            });
        }

        private static UserRole ToModel(UserDto user) =>
            user.RoleDto switch
            {
                UserRoleDto.Supervisor => UserRole.Supervisor,
                UserRoleDto.Worker => UserRole.Worker,
                UserRoleDto.Customer => UserRole.OrganizationOwner,
                UserRoleDto.CustomerRepresentative => UserRole.OrganizationMember,
                _ => throw new ArgumentOutOfRangeException()
            };

        [HttpPost("{userId:long}/organization")]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto>>> CreateOrganization(
            [FromRoute] long userId,
            [FromBody] OrganizationDto organization)
        {
            var user = await _userService.GetUserByIdOrNull(userId);
            if (user == null)
                return UserNotFound(userId);
            var createdOrganization = await _organizationService.CreateOrganization(user, organization.Name);
            return Created(Resources.Organizations.AppendPathSegment(createdOrganization.Id),
                new PersistedOrganizationDto
                {
                    Id = createdOrganization.Id,
                    Name = createdOrganization.Name
                });
        }

        [HttpGet]
        private NotFoundObjectResult UserNotFound(long userId) =>
            NotFound($"User by not found by id {userId}", "users.userNotFound");
    }

    public class PersistedUserDto
    {
    }

    public class OrganizationDto
    {
        public string Name { get; init; } = null!;
    }
}