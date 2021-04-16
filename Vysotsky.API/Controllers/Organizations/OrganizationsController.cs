using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Organizations;
using Vysotsky.API.Dto.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using OrganizationDto = Vysotsky.API.Dto.Organizations.OrganizationDto;

namespace Vysotsky.API.Controllers.Organizations
{
    [Route(Resources.Organizations)]
    public class OrganizationsController : ApiController
    {
        private readonly IOrganizationService organizationService;
        private readonly IRoomService roomService;
        private readonly IUserService userService;
        private readonly ICurrentUserProvider currentUserProvider;

        public OrganizationsController(IOrganizationService organizationService,
            IRoomService roomService,
            IUserService userService,
            ICurrentUserProvider currentUserProvider)
        {
            this.organizationService = organizationService;
            this.roomService = roomService;
            this.userService = userService;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet("{organizationId:long}/rooms")]
        public async Task<ActionResult<ApiResponse<OrganizationBuildingDto[]>>> GetOrganizationRooms(
            long organizationId)
        {
            var organization = await organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
            {
                return OrganizationNotFound(organizationId);
            }

            if (!currentUserProvider.CanReadOrganization(organizationId))
            {
                return NotAuthorized("Only organization member can read organization data",
                    "organization.read.notAuthorized");
            }

            var buildings = await roomService.GetOrganizationBuildingsAsync(organization);
            return Ok(buildings.Select(b => b.ToDto()));
        }

        [HttpGet("{organizationId:long}/representatives")]
        public async Task<ActionResult<ApiResponse<PersistedUserDto>>> GetAllRepresentatives(long organizationId)
        {
            var organization = await organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization != null)
            {
                var currentUser = currentUserProvider.CurrentUser;
                if (currentUser.Role != UserRole.Supervisor
                    || currentUser.Role != UserRole.SuperUser
                    || currentUser.OrganizationId != organizationId)
                {
                    organization = null;
                }
            }

            if (organization == null)
            {
                return OrganizationNotFound(organizationId);
            }

            var users = await userService.GetAllOrganizationMembersAsync(organization);
            return Ok(users.Select(u => u.ToDto()));
        }

        [HttpGet("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto>>> GetOrganization(
            [FromRoute] long organizationId)
        {
            var organization = await organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
            {
                return OrganizationNotFound(organizationId);
            }

            if (!currentUserProvider.CanReadOrganization(organizationId))
            {
                return NotAuthorized("Only organization member can read organization data",
                    "organization.read.notAuthorized");
            }

            return Ok(organization.ToDto());
        }

        private NotFoundObjectResult OrganizationNotFound(long organizationId) => NotFound($"Organization by id {organizationId} not found", "organizations.organizationNotFound");

        [HttpPut("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrganization([FromRoute] long organizationId,
            [FromBody] OrganizationDto organizationDtoProperties)
        {
            var organization = await organizationService.GetOrganizationByIdOrNullAsync(organizationId);
            if (organization == null)
            {
                return OrganizationNotFound(organizationId);
            }

            if (!currentUserProvider.CanWriteOrganization(organizationId))
            {
                return NotAuthorized("Only organization owner can edit organization",
                    "organizations.edit.notAuthorized");
            }

            var newOrganization = organization with { Name = organizationDtoProperties.Name };
            await organizationService.UpdateOrganization(newOrganization);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PersistedOrganizationDto[]>>> GetAllOrganizations()
        {
            var organizations = await organizationService.GetAllOrganizations();
            return Ok(organizations.Select(o => o.ToDto()));
        }
    }
}
