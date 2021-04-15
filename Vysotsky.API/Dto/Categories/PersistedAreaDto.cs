using Vysotsky.API.Dto.Images;

namespace Vysotsky.API.Dto.Categories
{
    public class PersistedAreaDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public PersistedImageDto Image { get; init; }
    }
}