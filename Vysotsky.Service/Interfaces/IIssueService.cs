using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IIssueService
    {
        Task<Area?> GetAreaByIdOrNull(long id);
        Task<Issue> CreateIssueAsync(string title, string description, Area area, Room room, User author);
        Task<Issue?> GetIssueByIdOrNullAsync(long issueId);
        Task<Issue> MoveIssueToNeedInformation(Issue issue, User supervisor, string message);
    }
}