using System.Threading.Tasks;

namespace Vysotsky.API.Hubs
{
    public interface INotificationHubClient
    {
        Task ReceiveNotification<T>(string title, string text, T payload);
    }
}