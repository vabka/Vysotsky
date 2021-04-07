using System.Collections.Generic;

namespace Vysotsky.Data
{
    public class TaskHistoryRecord : Entity
    {
        public long Task { get; init; }
        public TaskAction Action { get; init; }
        public Dictionary<string, string>? Extension { get; init; }
    }
}