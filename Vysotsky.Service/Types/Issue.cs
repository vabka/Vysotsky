using System;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class Issue
    {
        public long Id { get; init; }
        public long CategoryId { get; init; }
        public string Description { get; init; } = null!;
        public Room Room { get; init; } = null!;
        public IssueStatus Status { get; init; }
        public string Title { get; init; } = null!;
        public long Version { get; init; }
        public long AuthorId { get; init; }
        public long? SupervisorId { get; init; }
        public long? WorkerId { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
    }
}
