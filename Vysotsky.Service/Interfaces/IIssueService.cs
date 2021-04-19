using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IIssueService
    {
        Task<Area?> GetAreaByIdOrNull(long id);
        Task<Issue> CreateIssueAsync(string title, string description, Area area, Room room, User author);
        Task<Issue?> GetIssueByIdOrNullAsync(long issueId);
        Task<Issue> MoveIssueToNeedInformationAsync(Issue issue, User supervisor, string message);
        Task<Issue> TakeToWorkAsync(Issue issue, User supervisor, User worker, Category newCategory);
        Task<(int Total, IEnumerable<ShortIssue> Issues)> GetIssuesToShowUser(User user, DateTimeOffset maxDate, int limit, int offset);
        Task<IEnumerable<Issue>> GetIssueHistoryAsync(Issue issue);
    }
}
