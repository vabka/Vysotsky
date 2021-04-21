using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vysotsky.API.Dto.Chats;
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

        public async Task SendMessageToSupport(MessageContentDto content)
        {
            if (currentUserProvider.CurrentUser.IsCustomer())
            {
                var chat = await chatService.GetConversationByUserAsync(currentUserProvider.CurrentUser);
                await Send(content, chat);
            }
        }

        public async Task SendMessageToCustomer(long customerId, MessageContentDto content)
        {
            if (currentUserProvider.CurrentUser.IsSupervisor())
            {
                var chat = await chatService.GetConversationByIdOrNullAsync(customerId);
                if (chat != null)
                {
                    await Send(content, chat);
                }
            }
        }

        private async Task Send(MessageContentDto content, Conversation conversation) =>
            await chatService.SendAsync(currentUserProvider.CurrentUser, conversation,
                new MessageContent {Text = content.Text!});
    }
}
