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

        private readonly VysotskyDataConnection _db;

        public IssueService(VysotskyDataConnection vysotskyDataConnection) =>
            _db = vysotskyDataConnection;

        public async Task<Area?> GetAreaByIdOrNull(long id) =>
            await _db.Areas
                .Where(x => x.Id == id)
                .Select(x => new Area {Id = id})
                .SingleOrDefaultAsync();

        public async Task<Issue> CreateIssueAsync(string title, string description, Area area, Room room, User author)
        {
            var id = await _db.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
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
            await _db.Issues
                .Select(MapToIssue)
                .SingleAsync(i => i.Id == issueId && i.Version == version);

        public async Task<Issue?> GetIssueByIdOrNullAsync(long issueId) =>
            await _db.Issues
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
                    await using var transaction = await _db.BeginTransactionAsync();
                    await _db.IssueComments
                        .InsertAsync(() => new IssueCommentRecord
                        {
                            IssueId = issue.Id, AuthorId = supervisor.Id, Text = message
                        });
                    await _db.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(_db.Issues,
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
                    await _db.Issues
                        .Where(i => i.Id == issue.Id && i.Version == issue.Version)
                        .Take(1)
                        .InsertAsync(_db.Issues,
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

        private static readonly Expression<Func<IssueRecord, ShortIssue>> MapToShortIssue = record => new ShortIssue
        {
            Id = record.Id, Status = record.Status, Title = record.Title, CreatedAt = record.CreatedAt
        };

        public async Task<(int total, IEnumerable<ShortIssue>)> GetIssuesToShowUser(User user, DateTimeOffset maxDate,
            int limit,
            int offset)
        {
            var query = _db.Issues
                .Where(i => i.CreatedAt < maxDate)
                .OrderByDescending(i => i.CreatedAt)
                .ThenBy(i => i.Id)
                .AsQueryable();
            query = user switch
            {
                {Role: UserRole.SuperUser or UserRole.Supervisor}
                    => query,
                {Role: UserRole.Worker}
                    => query.Where(i => i.WorkerId == user.Id),
                {Role: UserRole.OrganizationOwner or UserRole.OrganizationMember}
                    => query
                        .InnerJoin(_db.Users, (l, r) => l.AuthorId == r.Id,
                            (i, u) => new {Issue = i, u.OrganizationId})
                        .Where(x => x.OrganizationId == user.OrganizationId)
                        .Select(x => x.Issue),
                _ => throw new InvalidOperationException()
            };
            query = GetActualVersions(query);
            var count = await query.CountAsync();
            var data = await query
                .Select(MapToShortIssue)
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
            return (count, data);
        }

        private static IQueryable<T> GetActualVersions<T>(IQueryable<T> query) where T : VersionedEntity =>
            query
                .Select(i => new
                {
                    Record = i, rn = Sql.Ext.RowNumber().Over().PartitionBy(i.Id).OrderByDesc(i.Version).ToValue()
                })
                .Where(i => i.rn == 1)
                .Select(i => i.Record);

        private static InvalidOperationException CannotMoveFromTerminalState() =>
            new("Cannot move from terminal state");
    }
}
