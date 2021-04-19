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
        private static readonly Expression<Func<IssueRecord, FullIssue>> MapToIssue = issue => new FullIssue
        {
            Id = issue.Id,
            Area =
                new Area {Id = issue.Area!.Id, Name = issue.Area.Name, Image = new Image {Id = issue.Area.ImageId}},
            Category =
                issue.Category != null
                    ? new Category {Id = issue.Category.Id, Name = issue.Category.Name, AreaId = issue.Category.AreaId}
                    : null,
            Description = issue.Description,
            Note = issue.Description,
            Room =
                new Room
                {
                    Id = issue.Room!.Id,
                    Name = issue.Room.Name,
                    Number = issue.Room.Number,
                    Status = issue.Room.Status,
                    OwnerId = issue.Room.OwnerId
                },
            Status = issue.Status,
            Title = issue.Title,
            Version = issue.Version,
            Author = new User
            {
                Id = issue.Author!.Id,
                Firstname = issue.Author.FirstName,
                LastName = issue.Author.LastName,
                Patronymic = issue.Author.Patronymic,
                Contacts = issue.Author.Contacts,
                Role = issue.Author.Role,
                Username = issue.Author.Username,
                OrganizationId = issue.Author.OrganizationId
            },
            SupervisorId = issue.SupervisorId,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            WorkerId = issue.WorkerId
        };

        private readonly VysotskyDataConnection db;

        public IssueService(VysotskyDataConnection vysotskyDataConnection) =>
            db = vysotskyDataConnection;

        public async Task<Area?> GetAreaByIdOrNull(long id) =>
            await db.Areas
                .Where(x => x.Id == id)
                .Select(x => new Area {Id = id})
                .SingleOrDefaultAsync();

        public async Task<FullIssue> CreateIssueAsync(string title, string description, Area area, Room room, User author)
        {
            var id = await db.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
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

        private async Task<FullIssue> GetIssueByIdWithSpecificVersion(long issueId, long version) =>
            await db.Issues
                .Select(MapToIssue)
                .SingleAsync(i => i.Id == issueId && i.Version == version);

        public async Task<FullIssue?> GetIssueByIdOrNullAsync(long issueId) =>
            await db.Issues
                .Where(x => x.Id == issueId)
                .GetActualVersions()
                .Select(MapToIssue)
                .SingleOrDefaultAsync();

        public async Task<FullIssue> MoveIssueToNeedInformationAsync(FullIssue issue, User supervisor, string message)
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

        public async Task<FullIssue> TakeToWorkAsync(FullIssue issue, User supervisor, User worker,
            Category newCategory)
        {
            switch (issue.Status)
            {
                case IssueStatus.New:
                case IssueStatus.NeedInfo:
                {
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
                                AreaId = newCategory.AreaId,
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

        public async Task<(int Total, IEnumerable<ShortIssue> Issues)> GetIssuesToShowUser(User user,
            DateTimeOffset maxDate,
            int limit,
            int offset)
        {
            var query = db.Issues
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
                        .InnerJoin(db.Users, (l, r) => l.AuthorId == r.Id,
                            (i, u) => new {Issue = i, u.OrganizationId})
                        .Where(x => x.OrganizationId == user.OrganizationId)
                        .Select(x => x.Issue),
                _ => throw new InvalidOperationException()
            };
            query = query.GetActualVersions();
            var count = await query.CountAsync();
            var data = await query
                .Select(MapToShortIssue)
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
            return (count, data);
        }

        public async Task<IEnumerable<FullIssue>> GetIssueHistoryAsync(FullIssue issue) =>
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
