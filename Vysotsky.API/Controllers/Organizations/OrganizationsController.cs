using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Controllers.Organizations.Dto;
using Vysotsky.API.Controllers.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using OrganizationDto = Vysotsky.API.Controllers.Organizations.Dto.OrganizationDto;

namespace Vysotsky.API.Controllers.Organizations
{
    [Route(Resources.Organizations)]
    public class OrganizationsController : ApiController
    {
        private readonly IOrganizationService _organizationService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public OrganizationsController(IOrganizationService organizationService,
            IRoomService roomService,
            IUserService userService,
            ICurrentUserProvider currentUserProvider)
        {
            _organizationService = organizationService;
            _roomService = roomService;
            _userService = userService;
            _currentUserProvider = currentUserProvider;
        }

        [HttpGet("{organizationId:long}/rooms")]
        public async Task<ActionResult<ApiResponse<OrganizationBuildingDto[]>>> GetOrganizationRooms(
            long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
            if (!_currentUserProvider.CanReadOrganization(organizationId))
                return NotAuthorized("Only organization member can read organization data",
                    "organization.read.notAuthorized");
            var buildings = await _roomService.GetOrganizationBuildings(organization);
            return Ok(buildings.Select(b => new OrganizationBuildingDto
            {
                Id = b.Id,
                Name = b.Name,
                Floors = b.Floors.Select(f => new OrganizationFloorDto
                {
                    Id = f.Id,
                    Number = f.Number,
                    Rooms = f.Rooms.Select(r => new OrganizationRoomDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Number = r.Number
                    }).ToArray()
                }).ToArray()
            }).ToArray());
        }

        [HttpGet("{organizationId:long}/representatives")]
        public async Task<ActionResult<ApiResponse<RepresentativeDto>>> GetAllRepresentatives(long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization != null)
            {
                var currentUser = _currentUserProvider.CurrentUser;
                if (currentUser?.Role != UserRole.Supervisor
                    || currentUser.Role != UserRole.SuperUser
                    || currentUser.OrganizationId != organizationId)
                    organization = null;
            }

            if (organization == null)
                return OrganizationNotFound(organizationId);
            var users = await _userService.GetAllOrganizationMembersAsync(organization);
            return Ok(users.Select(u => new RepresentativeDto
            {
                Id = u.Id,
                Username = u.Username,
                Name = new PersonName
                {
                    FirstName = u.Firstname,
                    LastName = u.LastName,
                    Patronymic = u.Patronymic
                }
            }).ToArray());
        }

        [HttpGet("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto>>> GetOrganization(
            [FromRoute] long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
            if (!_currentUserProvider.CanReadOrganization(organizationId))
                return NotAuthorized("Only organization member can read organization data",
                    "organization.read.notAuthorized");
            return Ok(new PersistedOrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name
            });
        }

        private NotFoundObjectResult OrganizationNotFound(long organizationId) =>
            NotFound($"Organization by id {organizationId} not found", "organizations.organizationNotFound");

        [HttpPut("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrganization([FromRoute] long organizationId,
            [FromBody] OrganizationDto organizationDtoProperties)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
            if (!_currentUserProvider.CanWriteOrganization(organizationId))
                return NotAuthorized("Only organization owner can edit organization",
                    "organizations.edit.notAuthorized");
            var newOrganization = organization with {Name = organizationDtoProperties.Name};
            await _organizationService.UpdateOrganization(newOrganization);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto[]>>> GetAllOrganizations()
        {
            var organizations = await _organizationService.GetAllOrganizations();
            return Ok(organizations.Select(o => new PersistedOrganizationDto
            {
                Id = o.Id,
                Name = o.Name
            }));
        }
    }

    public class RepresentativeDto
    {
        public long Id { get; set; }
        public string Username { get; init; } = null!;
        public PersonName Name { get; init; } = null!;
    }

    public class OrganizationBuildingDto
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
        public OrganizationFloorDto[] Floors { get; init; } = Array.Empty<OrganizationFloorDto>();
    }

    public class OrganizationFloorDto
    {
        public long Id { get; init; }
        public string Number { get; init; } = null!;
        public OrganizationRoomDto[] Rooms { get; init; } = Array.Empty<OrganizationRoomDto>();
    }

    public class OrganizationRoomDto
    {
        public long Id { get; init; }
        public string? Name { get; init; }
        public string? Number { get; init; }
    }
}