namespace Vysotsky.API.Dto.Issues
{
    public class IssueDto
    {
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public long CategoryId { get; init; }
        public long RoomId { get; init; }
    }
}
