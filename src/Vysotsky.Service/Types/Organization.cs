namespace Vysotsky.Service.Types
{
    public record Organization
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
    }
}
