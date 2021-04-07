namespace Vysotsky.Data
{
    public class InternalTaskCategory : Entity
    {
        public long TaskCategoryId { get; init; }
        public string Name { get; init; } = null!;
    }
}