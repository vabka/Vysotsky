using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class IssueService : IIssueService
    {
        private readonly VysotskyDataConnection _vysotskyDataConnection;

        public IssueService(VysotskyDataConnection vysotskyDataConnection)
        {
            _vysotskyDataConnection = vysotskyDataConnection;
        }

        public async Task<Area?> GetAreaByIdOrNull(long id) =>
            await _vysotskyDataConnection.Areas
                .Where(x => x.Id == id)
                .Select(x => new Area
                {
                    Id = id
                })
                .SingleOrDefaultAsync();

        public async Task<Issue> CreateIssueAsync(string title, string description, Area area, Room room, User author)
        {
            var id = await _vysotskyDataConnection.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
            {
                Title = title,
                Description = description,
                AreaId = area.Id,
                AuthorId = author.Id,
                RoomId = room.Id,
                Note = "",
                Status = IssueStatus.New
            });
            return (await GetIssueByIdOrNullAsync(id))!;
        }

        public async Task<Issue?> GetIssueByIdOrNullAsync(long issueId) =>
            await _vysotskyDataConnection.Issues
                .Where(x => x.Id == issueId)
                .OrderByDescending(x => x.Version)
                .Select(i => new Issue
                {
                    Id = i.Id,
                    Version = i.Version,
                    Status = i.Status,
                    Title = i.Title,
                    Description = i.Description,
                    Note = i.Note,
                    AreaId = i.AreaId,
                    CategoryId = i.CategoryId,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    RoomId = i.RoomId,
                    SupervisorId = i.SupervisorId,
                    WorkerId = i.WorkerId,
                    AuthorId = i.AuthorId
                })
                .Take(1)
                .SingleOrDefaultAsync();

        public async Task<Issue> MoveIssueToNeedInformation(Issue issue, User supervisor, string message)
        {
            switch (issue.Status)
            {
                case IssueStatus.InProgress:
                case IssueStatus.New:
                {
                    await using var transaction = await _vysotskyDataConnection.BeginTransactionAsync();

                    await _vysotskyDataConnection.IssueComments.InsertAsync(() => new IssueCommentRecord
                    {
                        IssueId = issue.Id,
                        AuthorId = supervisor.Id,
                        Text = message
                    });
                    await _vysotskyDataConnection.Issues.InsertAsync(
                        () => new IssueRecord
                        {
                            Id = issue.Id,
                            Version = issue.Version + 1,
                            Status = IssueStatus.NeedInfo,
                            SupervisorId = supervisor.Id,
                            Title = issue.Title,
                            Description = issue.Description,
                            Note = issue.Note,
                            AuthorId = issue.AuthorId,
                            AreaId = issue.AreaId,
                            CategoryId = issue.CategoryId,
                            RoomId = issue.RoomId,
                            WorkerId = issue.WorkerId,
                            CreatedAt = issue.CreatedAt
                        });
                    await transaction.CommitAsync();
                }
                    issue = (await GetIssueByIdOrNullAsync(issue.Id))!;
                    break;
                case IssueStatus.Completed:
                case IssueStatus.Accepted:
                case IssueStatus.NeedInfo:
                case IssueStatus.Closed:
                case IssueStatus.Rejected:
                case IssueStatus.CancelledByCustomer:
                    throw new InvalidOperationException("Cannot move from terminal state");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return issue;
        }
    }
}