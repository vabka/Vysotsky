using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public class Category
    {
        public long Id { get; init; }
        public Area Area { get; init; }
        public string Name { get; init; }
    }
}