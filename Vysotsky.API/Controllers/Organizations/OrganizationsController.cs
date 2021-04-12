using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Controllers.Organizations.Dto;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;
using OrganizationToCreate = Vysotsky.API.Controllers.Organizations.Dto.Organization;

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

        [HttpGet("{organizationId:long}")]
        public async Task<ActionResult<ApiResponse<PersistedOrganization>>> GetOrganization([FromRoute] long organizationId)
        {
            var organization = await _organizationService.GetOrganizationByIdOrNull(organizationId);
            if (organization == null)
                return NotFound($"Organization by id {organizationId} not found", "organizations.organizationNotFound");
            return Ok(new PersistedOrganization
            {
                Id = organization.Id,
                Name = organization.Name 
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedOrganization>>> CreateOrganization(
            OrganizationToCreate organization)
        {
            var createdOrganization = await _organizationService.CreateOrganization(organization.Name);
            return Created(Resources.Organizations.AppendPathSegment(createdOrganization.Id),
                new PersistedOrganization
                {
                    Id = createdOrganization.Id,
                    Name = createdOrganization.Name
                });
        }
    }
}