using System.Threading.Tasks;

namespace Vysotsky.API.Hubs
{
    public interface INotificationHubClient
    {
        Task ReceiveEvent<TPayload>(string type, TPayload payload);
    }
}
