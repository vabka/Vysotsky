using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Vysotsky.API.Hubs;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Infrastructure
{
    public class SignalREventBus : IEventBus
    {
        private readonly IHubContext<NotificationHub, INotificationHubClient> notificationHub;

        public SignalREventBus(IHubContext<NotificationHub, INotificationHubClient> notificationHub) =>
            this.notificationHub = notificationHub;

        public async Task PushAsync<TEvent>(TEvent data) where TEvent : Event =>
            //TODO Отправлять не всем
            await notificationHub.Clients.All.ReceiveEvent(typeof(TEvent).Name, data);
    }
}
