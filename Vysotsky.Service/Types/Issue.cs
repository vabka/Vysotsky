using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class Issue
    {
        public long Id { get; init; }
        public IssueStatus Status { get; init; }
    }
}