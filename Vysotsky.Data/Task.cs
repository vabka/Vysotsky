namespace Vysotsky.Data
{
    public class Task : Entity
    {
        public long TaskCategoryId { get; init; }
        public long? InternalTaskCategoryId { get; init; }
        public TaskStatus Status { get; init; }
        public string Description { get; init; } = null!;
        public long? SupervisorId { get; init; }
        public long? WorkerId { get; init; }
        public string Note { get; init; } = "";
    }
}