using Vysotsky.API.Dto.Images;

namespace Vysotsky.API.Dto.Categories
{
    public record PersistedAreaDto(long Id, string Name, PersistedImageDto Image);
}
