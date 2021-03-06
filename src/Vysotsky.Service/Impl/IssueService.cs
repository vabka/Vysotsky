using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Data.Entities.Abstraction;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class IssueService : IIssueService
    {
        private readonly VysotskyDataConnection db;
        private readonly IEventBus eventBus;

        private static readonly Expression<Func<IssueRecord, Issue>> MapToIssue = issue => new Issue
        {
            Id = issue.Id,
            CategoryId = issue.CategoryId,
            Description = issue.Description,
            Room = new Room
            {
                Id = issue.Room.Id,
                Number = issue.Room.Number,
                Status = issue.Room.Status,
                OwnerId = issue.Room.OwnerId
            },
            Status = issue.Status,
            Title = issue.Title,
            Version = issue.Version,
            AuthorId = issue.AuthorId,
            SupervisorId = issue.SupervisorId,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            WorkerId = issue.WorkerId,
            HasUnreadComments = issue.HasUnreadComments
        };

        public IssueService(VysotskyDataConnection vysotskyDataConnection, IEventBus eventBus)
        {
            db = vysotskyDataConnection;
            this.eventBus = eventBus;
        }


        public async Task<Issue> CreateIssueAsync(string title, string description, Category category, Room room,
            User author)
        {
            var id = await db.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
            {
                Version = 1,
                Title = title,
                Description = description,
                AuthorId = author.Id,
                RoomId = room.Id,
                CategoryId = category.Id,
                Note = "",
                Status = IssueStatus.New
            });
            _ = eventBus.PushAsync(new IssueCreated {Id = id,});
            return (await GetIssueByIdOrNullAsync(id))!;
        }

        private async Task<Issue> GetIssueByIdWithSpecificVersion(long issueId, long version) =>
            await db.Issues
                .Select(MapToIssue)
                .SingleAsync(i => i.Id == issueId && i.Version == version);


        public async Task<Issue?> GetIssueByIdOrNullAsync(long issueId) =>
            await db.Issues
                .Where(x => x.Id == issueId)
                .GetActualVersions()
                .Select(MapToIssue)
                .FirstOrDefaultAsync();

        public async Task<Issue> MoveIssueToNeedInformationAsync(Issue issue, User supervisor, string message)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                {
                    await using var transaction = await db.BeginTransactionAsync();
                    await db.IssueComments
                        .InsertAsync(() => new IssueCommentRecord
                        {
                            IssueId = issue.Id, AuthorId = supervisor.Id, Text = message
                        });
                    await db.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(db.Issues,
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

        public async Task<Issue> TakeToWorkAsync(Issue issue, User supervisor, User worker, string message)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                case IssueStatus.NeedInfo:
                {
                    await using var transaction = await db.BeginTransactionAsync();
                    await db.IssueComments
                        .InsertAsync(() => new IssueCommentRecord
                        {
                            IssueId = issue.Id, AuthorId = supervisor.Id, Text = message
                        });
                    await db.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(db.Issues,
                            i => new IssueRecord
                            {
                                Id = i.Id,
                                Version = i.Version + 1,
                                Status = IssueStatus.InProgress,
                                WorkerId = worker.Id,
                                SupervisorId = supervisor.Id,
                                Description = i.Description,
                                Note = i.Note,
                                Title = i.Title,
                                CategoryId = i.CategoryId,
                                AuthorId = i.AuthorId,
                                CreatedAt = i.CreatedAt,
                                UpdatedAt = DateTimeOffset.Now,
                                RoomId = i.RoomId
                            });
                    await transaction.CommitAsync();
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

        public Task<Issue> CompleteIssueAsync(Issue issue, User worker, string message) =>
            throw new NotImplementedException();

        public async Task<Issue> RejectIssueAsync(Issue issue, string message)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                case IssueStatus.NeedInfo:
                {
                    await using var transaction = await db.BeginTransactionAsync();
                    // await db.IssueComments
                    //     .InsertAsync(() => new IssueCommentRecord
                    //     {
                    //         IssueId = issue.Id, AuthorId = supervisor.Id, Text = message
                    //     });
                    await db.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(db.Issues,
                            i => new IssueRecord
                            {
                                Id = i.Id,
                                Version = i.Version + 1,
                                Status = IssueStatus.InProgress,
                                WorkerId = i.WorkerId,
                                SupervisorId = i.SupervisorId,
                                Description = i.Description,
                                Note = i.Note,
                                Title = i.Title,
                                CategoryId = i.CategoryId,
                                AuthorId = i.AuthorId,
                                CreatedAt = i.CreatedAt,
                                UpdatedAt = DateTimeOffset.Now,
                                RoomId = i.RoomId
                            });
                    await transaction.CommitAsync();
                    return await GetIssueByIdWithSpecificVersion(issue.Id, issue.Version + 1);
                }
                case IssueStatus.Accepted:
                case IssueStatus.InProgress:
                case IssueStatus.Completed:
                    throw new NotImplementedException();
                case IssueStatus.Rejected:
                    return issue;
                case IssueStatus.CancelledByCustomer:
                case IssueStatus.Closed:
                    throw CannotMoveFromTerminalState();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<IEnumerable<IssueComment>> GetCommentsAsync(Issue issue) =>
            await db.IssueComments
                .Where(x => x.IssueId == issue.Id)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new IssueComment
                {
                    Content = new MessageContent {Text = x.Text}, AuthorId = x.AuthorId, CreatedAt = x.CreatedAt
                })
                .ToArrayAsync();

        public async Task<(int Total, IEnumerable<ShortIssue> Issues)> GetIssuesToShowUser(User user,
            DateTimeOffset maxDate,
            int limit,
            int offset)
        {
            var query = db.Issues
                .Where(i => i.CreatedAt < maxDate)
                .AsQueryable();
            query = user switch
            {
                {Role: UserRole.SuperUser or UserRole.Supervisor}
                    => query
                        .GetActualVersions()
                        .OrderBy(x =>
                            x.Status == IssueStatus.New
                                ? 0
                                : x.Status == IssueStatus.NeedInfo
                                    ? 1
                                    : x.Status == IssueStatus.Completed
                                        ? 2
                                        : 3
                        ),
                {Role: UserRole.Worker}
                    => query.Where(i => i.WorkerId == user.Id)
                        .OrderBy(i => i.Status == IssueStatus.InProgress ? 0 : 1),
                {Role: UserRole.OrganizationOwner or UserRole.OrganizationMember}
                    => query
                        .InnerJoin(db.Users, (l, r) => l.AuthorId == r.Id,
                            (i, u) => new {Issue = i, u.OrganizationId})
                        .Where(x => x.OrganizationId == user.OrganizationId)
                        .Select(x => x.Issue)
                        .GetActualVersions()
                        .OrderBy(x => x.Status == IssueStatus.NeedInfo
                            ? 0
                            : x.Status == IssueStatus.Closed ||
                              x.Status == IssueStatus.Completed ||
                              x.Status == IssueStatus.Rejected ||
                              x.Status == IssueStatus.CancelledByCustomer ||
                              x.Status == IssueStatus.Accepted
                                ? 2
                                : 1
                        ),
                _ => throw new InvalidOperationException()
            };
            var count = await query.CountAsync();
            var data = await query
                .ThenOrByDescending(i => i.CreatedAt)
                .ThenByDescending(i => i.Id)
                .InnerJoin(db.Rooms, (i, r) => i.RoomId == r.Id, (i, r) => new {Issue = i, Room = r})
                .Select(x => new ShortIssue
                {
                    Id = x.Issue.Id,
                    Status = x.Issue.Status,
                    Title = x.Issue.Title,
                    CreatedAt = x.Issue.CreatedAt,
                    Room = new Room
                    {
                        Id = x.Room.Id, Number = x.Room.Number, Status = x.Room.Status, OwnerId = x.Room.OwnerId
                    },
                    HasUnreadComments = x.Issue.HasUnreadComments
                })
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
            return (count, data);
        }

        public async Task<IEnumerable<Issue>> GetIssueHistoryAsync(Issue issue) =>
            await db.Issues
                .Where(i => i.Id == issue.Id)
                .OrderBy(i => i.Version)
                .Select(MapToIssue)
                .ToArrayAsync();


        private static InvalidOperationException CannotMoveFromTerminalState() =>
            new("Cannot move from terminal state");
    }

    public static class VersionedEntityExtensions
    {
        public static IQueryable<T> GetActualVersions<T>(this IQueryable<T> query) where T : VersionedEntity =>
            query
                .Select(i => new
                {
                    Record = i, rn = Sql.Ext.RowNumber().Over().PartitionBy(i.Id).OrderByDesc(i.Version).ToValue()
                })
                .Where(i => i.rn == 1)
                .Select(i => i.Record);
    }
}
