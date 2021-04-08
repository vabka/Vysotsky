using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    [Column("issue_history")]
    public class IssueHistoryRecord : Entity
    {
        [Column("issue_id")]
        public long IssueId { get; init; }
        [Column("event")]
        public IssueEvent Event { get; init; }
        [Column("extension")]
        public Dictionary<string, string>? Extension { get; init; }
    }
}
