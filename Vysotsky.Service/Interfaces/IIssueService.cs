using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IIssueService
    {
        Task<Issue> CreateIssueAsync(string title, string description, Category category, Room room,
            User author);

        Task<Issue?> GetIssueByIdOrNullAsync(long issueId);
        Task<(int Total, IEnumerable<ShortIssue> Issues)> GetIssuesToShowUser(User user, DateTimeOffset maxDate,
            int limit, int offset);

        Task<IEnumerable<Issue>> GetIssueHistoryAsync(Issue issue);


        Task<Issue> MoveIssueToNeedInformationAsync(Issue issue, User supervisor, string message);
        Task<Issue> TakeToWorkAsync(Issue issue, User supervisor, User worker, string message);
        Task<Issue> CompleteIssueAsync(Issue issue, User worker, string message);

        Task<Issue> RejectIssueAsync(Issue issue, string message);
        Task<IEnumerable<IssueComment>> GetCommentsAsync(Issue issue);
    }

    public class IssueComment
    {
        public DateTimeOffset CreatedAt { get; init; }
        public long AuthorId { get; init; }
        public MessageContent Content { get; init; } = null!;
    }
}
