using System;
using Vysotsky.API.Dto.Buildings;

namespace Vysotsky.API.Dto.Issues
{
    public class PersistedIssueDto
    {
        public long Id { get; init; }
        public long Version { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public IssueStatusDto Status { get; init; }
        public long AuthorId { get; init; }
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public long CategoryId { get; init; }
        public PersistedRoomDto Room { get; init; } = null!;
    }
}
