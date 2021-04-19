using Vysotsky.API.Dto.Buildings;
using Vysotsky.API.Dto.Categories;

namespace Vysotsky.API.Dto.Issues
{
    public class PersistedIssueDto
    {
        public long Id { get; init; }
        public long Version { get; init; }
        public IssueStatusDto Status { get; init; }
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public PersistedAreaDto Area { get; init; } = null!;
        public PersistedCategoryDto? Category { get; init; }
        public PersistedRoomDto Room { get; init; } = null!;
    }
}
