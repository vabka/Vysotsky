using System;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class ShortIssue
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public DateTimeOffset CreatedAt { get; init; }
        public IssueStatus Status { get; init; }
    }
}
