using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IEventBus
    {
        Task PushAsync<TEvent>(TEvent data) where TEvent : Event;
    }
    public abstract class Event
    {
    }
    public class IssueCreatedEvent : Event
    {
        public long IssueId { get; init; }
        public long AuthorId { get; init; }
    }
    public interface INotificationService
    {
        Task SendNotificationToSupervisors(Notification notification);
        Task SendNotificationToOrganization(Notification notification, Organization organization);
        Task SendNotificationToUser(Notification notification, User user);
        Task SendNotificationToIssueSubscribers(Notification notification, Issue issue);
    }
}
