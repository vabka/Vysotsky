namespace Vysotsky.API.Controllers.Organizations.Dto
{
    public class PersistedOrganization
    {
        public long Id { get; init; }

        public string Name { get; init; } = null!;
    }

    public class Organization
    {
        public string Name { get; init; } = null!;
    }
}