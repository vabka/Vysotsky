namespace Vysotsky.Data
{
    public class Organization : Entity
    {
        public string Name { get; init; } = null!;
        public long OwnerId { get; init; }
    }
}