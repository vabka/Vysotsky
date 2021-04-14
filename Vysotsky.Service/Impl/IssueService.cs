using System;
using System.Collections.Generic;
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
            return new Issue
            {
                Id = id
            };
        }

        public async Task<Issue?> GetIssueByIdOrNullAsync(long issueId) =>
            await _vysotskyDataConnection.Issues.Where(x => x.Id == issueId)
                .Select(i => new Issue
                {
                    Id = i.Id,
                    Status = i.Status
                })
                .SingleOrDefaultAsync();

        public async Task<Issue> MoveIssueToNeedInformation(Issue issue, User supervisor, string message)
        {
            await using var transaction = await _vysotskyDataConnection.BeginTransactionAsync();
            switch (issue.Status)
            {
                case IssueStatus.InProgress:
                case IssueStatus.New:
                    await _vysotskyDataConnection.Issues.UpdateAsync(i => i.Id == issue.Id,
                        _ => new IssueRecord
                        {
                            Status = IssueStatus.NeedInfo,
                            SupervisorId = supervisor.Id
                        });
                    await _vysotskyDataConnection.IssueComments.InsertAsync(() => new IssueCommentRecord
                    {
                        IssueId = issue.Id,
                        AuthorId = supervisor.Id,
                        Text = message
                    });
                {
                    var extension = new Dictionary<string, string>
                    {
                        {"NewSupervisor", supervisor.Username}
                    };
                    await _vysotskyDataConnection.IssueHistory.InsertAsync(() => new IssueHistoryRecord
                    {
                        IssueId = issue.Id,
                        Event = IssueEvent.SupervisorChanged,
                        Extension = extension
                    });
                }
                {
                    var extension = new Dictionary<string, string>
                    {
                        {"NewState", IssueStatus.NeedInfo.ToString()}
                    };
                    await _vysotskyDataConnection.IssueHistory.InsertAsync(() => new IssueHistoryRecord
                    {
                        IssueId = issue.Id,
                        Event = IssueEvent.StatusChanged,
                        Extension = extension
                    });
                }
                    issue = (await GetIssueByIdOrNullAsync(issue.Id))!;
                    await transaction.CommitAsync();
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