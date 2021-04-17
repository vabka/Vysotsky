using Vysotsky.API.Dto.Images;

namespace Vysotsky.API.Dto.Categories
{
    public record PersistedAreaDto(long id, string name, PersistedImageDto image);
}
