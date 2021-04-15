namespace Vysotsky.Service.Types
{
    public class Category
    {
        public long Id { get; init; }
        public Area Area { get; init; } = null!;
        public string Name { get; init; } = null!;
    }
}