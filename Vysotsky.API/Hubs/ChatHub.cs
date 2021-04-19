using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHubClient>
    {
        private readonly ICurrentUserProvider currentUserProvider;

        private readonly IChatService chatService;

        public ChatHub(ICurrentUserProvider currentUserProvider, IChatService chatService)
        {
            this.currentUserProvider = currentUserProvider;
            this.chatService = chatService;
        }

        public async Task SendMessageToSupport(string text)
        {
            if (currentUserProvider.IsCustomer())
            {
                var chat = await chatService.GetChatByUserAsync(currentUserProvider.CurrentUser);
                await SendText(text, chat);
            }
        }

        public async Task SendMessageToCustomer(long customerId, string text)
        {
            if (currentUserProvider.IsSupervisor())
            {
                var chat = await chatService.GetChatByIdOrNullAsync(customerId);
                if (chat != null)
                {
                    await SendText(text, chat);
                }
            }
        }

        private async Task SendText(string text, Chat chat) =>
            await chatService.SendAsync(currentUserProvider.CurrentUser, chat,
                new MessageContent {Text = text});
    }
}