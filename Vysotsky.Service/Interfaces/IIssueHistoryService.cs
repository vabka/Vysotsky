using System.Threading.Tasks;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IIssueHistoryService
    {
        Task<IssueStatusChangedEvent> CreateStateChangedEvent(Issue issue, IssueStatus newStatus);
        Task<IssueDescriptionChangedEvent> CreateDescriptionChangedEvent(Issue issue);
        Task<IssueSupervisorChangedEvent> CreateSupervisorChangedEvent(Issue issue, User supervisor);
        Task<IssueWorkerChangedEvent> CreateWorkerChangedEvent(Issue issue, User worker);
    }
}