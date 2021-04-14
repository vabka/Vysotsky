using System.Collections.Generic;
using LinqToDB;
using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    [Column("issue_history")]
    public class IssueHistoryRecord : Entity
    {
        [Column("issue_id")] public long IssueId { get; init; }
        [Column("event", DbType = "issue_event")] public IssueEvent Event { get; init; }
        [Column("extension", DataType = DataType.BinaryJson)] public Dictionary<string, string>? Extension { get; init; }
    }

    public enum IssueEvent
    {
        [MapValue("StatusChanged")] StatusChanged,
        [MapValue("SupervisorChanged")] SupervisorChanged,
        [MapValue("WorkerChanged")] WorkerChanged,
        [MapValue("DescriptionChanged")] DescriptionChanged,
    }
}
