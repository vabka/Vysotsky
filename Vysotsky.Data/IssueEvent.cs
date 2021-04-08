using LinqToDB.Mapping;

namespace Vysotsky.Data
{
    public enum IssueEvent
    {
        [MapValue("StatusChanged")] StatusChanged,
        [MapValue("SupervisorChanged")] SupervisorChanged,
        [MapValue("WorkerChanged")] WorkerChanged,
        [MapValue("DescriptionChanged")] DescriptionChanged,
        [MapValue("CommentAdded")] CommentAdded
    }
}
