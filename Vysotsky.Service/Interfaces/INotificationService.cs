using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToSupervisors(Notification notification);
        Task SendNotificationToOrganization(Notification notification, Organization organization);
        Task SendNotificationToUser(Notification notification, User user);
        Task SendNotificationToIssueSubscribers(Notification notification, FullIssue issue);
    }
}
