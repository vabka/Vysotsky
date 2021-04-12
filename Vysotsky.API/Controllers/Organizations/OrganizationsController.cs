using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Controllers.Organizations.Dto;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;
using OrganizationDto = Vysotsky.API.Controllers.Organizations.Dto.Organization;

namespace Vysotsky.API.Controllers.Organizations
{
    [Route(Resources.Organizations)]
    public class OrganizationsController : ApiController
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("{organizationId:long}/rooms")]
        public async Task<ActionResult<ApiResponse<OrganizationBuildingDto[]>>> GetOrganizationRooms(
            long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNull(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
            var buildings = await _organizationService.GetOrganizationBuildings(organization);
            return Ok(buildings.Select(b => new OrganizationBuildingDto
            {
                Id = b.Id,
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

        [HttpGet("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto>>> GetOrganization(
            [FromRoute] long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNull(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
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
            [FromBody] OrganizationDto organizationProperties)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNull(organizationId);
            if (organization == null)
                return OrganizationNotFound(organizationId);
            var newOrganization = organization with {Name = organizationProperties.Name};
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