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

        [HttpPost("{username}/organization")]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto>>> CreateOrganization(string username,
            [FromBody] OrganizationDto organizationDto)
        {
            var user = await _userService.GetUserByUsernameOrNullAsync(username);
            if (user == null)
                return UserNotFound(username);
            if (user.OrganizationId != null)
                return Error("Organization is readonly", "users.organization.readonly", 405);
            if (user.Role != UserRole.OrganizationOwner)
                return NotFound("This user cant own organization", "user.organization.invalidUser");
            var createdOrganization = await _organizationService.CreateOrganization(user, organizationDto.Name);
            return Created(Resources.Organizations.AppendPathSegment(createdOrganization.Id),
                new PersistedOrganizationDto
                {
                    Id = createdOrganization.Id,
                    Name = createdOrganization.Name
                });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> RegisterUser(UserDto user)
        {
            var alreadyCreatedUser = await _userService.GetUserByUsernameOrNullAsync(user.Username);
            if (alreadyCreatedUser != null)
                return BadRequest("User with same username exists", "users.usernameExists");
            if (user.RoleDto == UserRoleDto.CustomerRepresentative)
                return BadRequest("Representative can be created only in organization",
                    "users.cannotCreateUnattachedRepresentative");
            var createdUser = await _userService.RegisterUserAsync(user.Username,
                user.Password,
                user.Name.FirstName,
                user.Name.LastName,
                user.Name.Patronymic,
                user.Contacts
                    .Select(c => new UserContact
                    {
                        Name = c.Name,
                        Value = c.Value,
                        Type = ToModel(c.Type)
                    })
                    .ToArray(),
                ToModel(user));
            return Created(Resources.Organizations.AppendPathSegment(createdUser.Username), new PersistedUserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Name = new PersonName
                {
                    FirstName = createdUser.Firstname,
                    LastName = createdUser.LastName,
                    Patronymic = createdUser.Patronymic
                },
                Contacts = createdUser.Contacts
                    .Select(c => new UserContactDto
                    {
                        Name = c.Name,
                        Value = c.Value,
                        Type = ToDto(c.Type)
                    })
                    .ToArray()
            });
        }

        private UserContactTypeDto ToDto(ContactType argType) => argType switch
        {
            ContactType.Phone => UserContactTypeDto.Phone,
            _ => throw new ArgumentOutOfRangeException(nameof(argType), argType, null)
        };

        private static ContactType ToModel(UserContactTypeDto c) =>
            c switch
            {
                UserContactTypeDto.Phone => ContactType.Phone,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static UserRole ToModel(UserDto user) =>
            user.RoleDto switch
            {
                UserRoleDto.Supervisor => UserRole.Supervisor,
                UserRoleDto.Worker => UserRole.Worker,
                UserRoleDto.Customer => UserRole.OrganizationOwner,
                UserRoleDto.CustomerRepresentative => UserRole.OrganizationMember,
                _ => throw new ArgumentOutOfRangeException()
            };

        [HttpGet]
        private NotFoundObjectResult UserNotFound(string username) =>
            NotFound($"User by not found by id {username}", "users.userNotFound");
    }

    public class PersistedUserDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
        public UserContactDto[] Contacts { get; init; } = Array.Empty<UserContactDto>();
    }

    public class OrganizationDto
    {
        public string Name { get; init; } = null!;
    }
}
