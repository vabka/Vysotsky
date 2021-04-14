
using System;

namespace Vysotsky.API.Dto.Organizations
{
    public class OrganizationDto
    {
        public string Name { get; init; } = null!;
        public long[] Rooms { get; init; } = Array.Empty<long>();

    }
}