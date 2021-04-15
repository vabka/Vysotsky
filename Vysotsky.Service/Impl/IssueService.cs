using System;
using System.Linq;
using System.Linq.Expressions;
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
        private static readonly Expression<Func<IssueRecord, Issue>> MapToIssue = i => new Issue
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
        };

        private readonly VysotskyDataConnection vysotskyDataConnection;

        public IssueService(VysotskyDataConnection vysotskyDataConnection)
        {
            this.vysotskyDataConnection = vysotskyDataConnection;
        }

        public async Task<Area?> GetAreaByIdOrNull(long id) =>
            await this.vysotskyDataConnection.Areas
                .Where(x => x.Id == id)
                .Select(x => new Area
                {
                    Id = id
                })
                .SingleOrDefaultAsync();

        public async Task<Issue> CreateIssueAsync(string title, string description, Area area, Room room, User author)
        {
            var id = await this.vysotskyDataConnection.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
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

        private async Task<Issue> GetIssueByIdWithSpecificVersion(long issueId, long version) =>
            await this.vysotskyDataConnection.Issues
                .Select(MapToIssue)
                .SingleAsync(i => i.Id == issueId && i.Version == version);

        public async Task<Issue?> GetIssueByIdOrNullAsync(long issueId) =>
            await this.vysotskyDataConnection.Issues
                .Where(x => x.Id == issueId)
                .OrderByDescending(x => x.Version)
                .Select(MapToIssue)
                .Take(1)
                .SingleOrDefaultAsync();

        public async Task<Issue> MoveIssueToNeedInformationAsync(Issue issue, User supervisor, string message)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                {
                    await using var transaction = await this.vysotskyDataConnection.BeginTransactionAsync();
                    await this.vysotskyDataConnection.IssueComments
                        .InsertAsync(() => new IssueCommentRecord
                        {
                            IssueId = issue.Id,
                            AuthorId = supervisor.Id,
                            Text = message
                        });
                    await this.vysotskyDataConnection.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(this.vysotskyDataConnection.Issues,
                            i => new IssueRecord
                            {
                                Id = i.Id,
                                Version = i.Version + 1,
                                Status = IssueStatus.NeedInfo,
                                SupervisorId = supervisor.Id,
                                Title = i.Title,
                                Description = i.Description,
                                Note = i.Note,
                                AuthorId = i.AuthorId,
                                AreaId = i.AreaId,
                                CategoryId = i.CategoryId,
                                RoomId = i.RoomId,
                                WorkerId = i.WorkerId,
                                CreatedAt = i.CreatedAt,
                                UpdatedAt = DateTimeOffset.Now
                            });
                    await transaction.CommitAsync();
                    return await GetIssueByIdWithSpecificVersion(issue.Id, issue.Version + 1);
                }
                case IssueStatus.InProgress:
                case IssueStatus.Completed:
                case IssueStatus.Accepted:
                case IssueStatus.NeedInfo:
                    throw new NotImplementedException();
                case IssueStatus.Closed:
                case IssueStatus.Rejected:
                case IssueStatus.CancelledByCustomer:
                    throw CannotMoveFromTerminalState();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<Issue> TakeToWorkAsync(Issue issue, User supervisor, User worker,
            Category newCategory)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                case IssueStatus.NeedInfo:
                {
                    await this.vysotskyDataConnection.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(this.vysotskyDataConnection.Issues, i => new IssueRecord
                        {
                            Id = i.Id,
                            Version = i.Version + 1,
                            Status = IssueStatus.InProgress,
                            WorkerId = worker.Id,
                            SupervisorId = supervisor.Id,
                            Description = i.Description,
                            Note = i.Note,
                            Title = i.Title,
                            AreaId = newCategory.Area.Id,
                            CategoryId = newCategory.Id,
                            AuthorId = i.AuthorId,
                            CreatedAt = i.CreatedAt,
                            UpdatedAt = DateTimeOffset.Now,
                            RoomId = i.RoomId
                        });
                    return await GetIssueByIdWithSpecificVersion(issue.Id, issue.Version + 1);
                }
                case IssueStatus.Accepted:
                case IssueStatus.InProgress:
                case IssueStatus.Completed:
                    throw new NotImplementedException();
                case IssueStatus.Rejected:
                case IssueStatus.CancelledByCustomer:
                case IssueStatus.Closed:
                    throw CannotMoveFromTerminalState();
                default:
                    throw new InvalidOperationException();
            }
        }

        private static InvalidOperationException CannotMoveFromTerminalState()
        {
            return new InvalidOperationException("Cannot move from terminal state");
        }
    }
}
