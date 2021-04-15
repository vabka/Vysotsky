namespace Vysotsky.Service.Types
{
    public class Floor
    {
        public long Id { get; init; }
        public string Number { get; init; } = null!;
        public long BuildingId { get; set; }
    }
}
