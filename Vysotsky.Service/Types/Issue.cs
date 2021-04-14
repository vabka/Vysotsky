using System;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class Issue
    {
        public long Id { get; init; }
        public long Version { get; init; }
        public IssueStatus Status { get; init; }
        public long RoomId { get; init; }
        public long AreaId { get; init; }
        public long? CategoryId { get; init; }
        public string Title { get; init; } = null!;
        public string Description { get; init; } = "";
        public long AuthorId { get; init; }
        public long? SupervisorId { get; init; }
        public long? WorkerId { get; init; }
        public string Note { get; init; } = "";
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
    }
}