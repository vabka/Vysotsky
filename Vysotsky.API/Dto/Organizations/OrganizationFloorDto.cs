using System;

namespace Vysotsky.API.Dto.Organizations
{
    public class OrganizationFloorDto
    {
        public long Id { get; init; }
        public string Number { get; init; } = null!;
        public OrganizationRoomDto[] Rooms { get; init; } = Array.Empty<OrganizationRoomDto>();
    }
}