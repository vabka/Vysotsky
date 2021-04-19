using System;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Types
{
    public class FullIssue
    {
        public long Id { get; init; }
        public long Version { get; init; }
        public IssueStatus Status { get; init; }
        public Room Room { get; init; } = null!;
        public Area Area { get; init; } = null!;
        public Category? Category { get; init; }
        public string Title { get; init; } = null!;
        public string Description { get; init; } = "";
        public User Author { get; init; } = null!;
        public long? SupervisorId { get; init; }
        public long? WorkerId { get; init; }
        public string Note { get; init; } = "";
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
    }
}
