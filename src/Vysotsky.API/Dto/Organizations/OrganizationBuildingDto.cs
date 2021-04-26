using System;
using System.Collections.Generic;

namespace Vysotsky.API.Dto.Organizations
{
    public class OrganizationBuildingDto
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
        public IEnumerable<OrganizationFloorDto> Floors { get; init; } = Array.Empty<OrganizationFloorDto>();
    }
}
