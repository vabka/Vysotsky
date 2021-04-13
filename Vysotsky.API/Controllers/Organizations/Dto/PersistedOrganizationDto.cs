namespace Vysotsky.API.Controllers.Organizations.Dto
{
    public class PersistedOrganizationDto
    {
        public long Id { get; init; }

        public string Name { get; init; } = null!;
    }

    public class Organization
    {
        public string Name { get; init; } = null!;
        public long OwnerId { get; init; }
    }
}