using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IIssueService
    {
        Task<Area?> GetAreaByIdOrNull(long id);
        Task<FullIssue> CreateIssueAsync(string title, string description, Area area, Room room, User author);
        Task<FullIssue?> GetIssueByIdOrNullAsync(long issueId);
        Task<FullIssue> MoveIssueToNeedInformationAsync(FullIssue issue, User supervisor, string message);
        Task<FullIssue> TakeToWorkAsync(FullIssue issue, User supervisor, User worker, Category newCategory);
        Task<(int Total, IEnumerable<ShortIssue> Issues)> GetIssuesToShowUser(User user, DateTimeOffset maxDate, int limit, int offset);
        Task<IEnumerable<FullIssue>> GetIssueHistoryAsync(FullIssue issue);
    }
}
