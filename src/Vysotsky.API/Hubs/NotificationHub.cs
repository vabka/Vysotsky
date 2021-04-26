using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Vysotsky.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub<INotificationHubClient>
    {
    }
}
