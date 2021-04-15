namespace Vysotsky.Data.Entities
{
    public record UserContact
    {
        public string Name { get; init; } = null!;
        public string Value { get; init; } = null!;
        public ContactType Type { get; init; }
    }
}
