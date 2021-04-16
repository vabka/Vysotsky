using System;

namespace Vysotsky.API.Dto.Issues
{
    public class ShortPersistedIssueDto
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public IssueStatusDto Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
