using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Vysotsky.API.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(long author, string text);
    }

    public class NotificationHub : Hub<object>
    {
    }

    public class ChatHub : Hub<IChatHubClient>
    {
        public Task SendMessage(string text) =>
            Task.CompletedTask;
    }
}
