namespace Vysotsky.API.Dto.Issues
{
    public class PersistedIssueDto
    {
        public long Id { get; init; }
        public IssueStatusDto Status { get; init; }
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public long AreaId { get; init; }
        public long RoomId { get; init; }
    }

    public enum IssueStatusDto
    {
        New,
    }
}