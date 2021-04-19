namespace Vysotsky.Service.Types
{
    public class Category
    {
        public long Id { get; init; }
        public long AreaId { get; init; }
        public string Name { get; init; } = null!;
    }
}
