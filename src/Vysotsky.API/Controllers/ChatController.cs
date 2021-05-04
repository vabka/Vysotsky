using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Chats;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Chats)]
    public class ChatController : ApiController
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IChatService chatService;

        public ChatController(ICurrentUserProvider currentUserProvider, IChatService chatService)
        {
            this.currentUserProvider = currentUserProvider;
            this.chatService = chatService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<WrappedListDto<ConversationDto>>>> GetChats()
        {
            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can read all conversations", "chats.notAuthorized");
            }

            var conversations = await chatService.GetAllStartedConversationsAsync();
            return Ok(conversations.Select(x => x.ToDto()).ToDto());
        }

        [HttpGet("support/messages")]
        public async Task<ActionResult<ApiResponse<PaginatedData<ChatMessageDto>>>> GetMessagesInSupportChat(
            [FromQuery] PaginationParameters paginationParameters,
            [FromQuery] SortingParameters sortingParameters)
        {
            var chat = await chatService.GetConversationByUserAsync(currentUserProvider.CurrentUser);
            var (total, messages) = await chatService.GetMessagesAsync(chat, paginationParameters.Until,
                sortingParameters.Ordering,
                paginationParameters.Skip,
                paginationParameters.Take);
            return Ok(PaginatedData.Create(paginationParameters, total, messages.Select(m => m.ToDto())));
        }

        [HttpPost("support/messages")]
        public async Task<ActionResult<ApiResponse<ChatMessageDto>>> SendMessageToSupport(
            [FromBody] MessageContentDto content)
        {
            var conversation = await chatService.GetConversationByUserAsync(currentUserProvider.CurrentUser);
            var msg = await chatService.SendAsync(currentUserProvider.CurrentUser, conversation,
                content.ToModel());
            return Ok(msg.ToDto());
        }

        [HttpPost("{conversationId:long}/messages")]
        public async Task<ActionResult<ApiResponse<ChatMessageDto>>> SendMessage([FromRoute] long conversationId,
            [FromBody] MessageContentDto content)
        {
            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can send messages to customers", "chats.notAuthorized");
            }

            var conversation = await chatService.GetConversationByIdOrNullAsync(conversationId);
            if (conversation == null)
            {
                return ConversationNotFound();
            }

            var msg = await chatService.SendAsync(currentUserProvider.CurrentUser, conversation,
                content.ToModel());
            return Ok(msg.ToDto());
        }

        private NotFoundObjectResult ConversationNotFound() => NotFound("Conversation not found", "chats.notFound");

        [HttpGet("{conversationId:long}/messages")]
        public async Task<ActionResult<ApiResponse<PaginatedData<ChatMessageDto>>>> GetMessagesInConversation(
            [FromRoute] long conversationId,
            [FromQuery] PaginationParameters paginationParameters,
            [FromQuery] SortingParameters sort
        )
        {
            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can read messages from customers", "chats.notAuthorized");
            }

            var conversation = await chatService.GetConversationByIdOrNullAsync(conversationId);
            if (conversation == null)
            {
                return ConversationNotFound();
            }

            var (total, messages) = await chatService.GetMessagesAsync(conversation, paginationParameters.Until,
                sort.Ordering,
                paginationParameters.Skip, paginationParameters.Take);
            return Ok(PaginatedData.Create(paginationParameters, total, messages.Select(m => m.ToDto())));
        }

        [HttpPost("support/read")]
        public async Task<ActionResult<ApiResponse>> MarkSupportChatAsRead()
        {
            var conversation = await chatService.GetConversationByUserAsync(currentUserProvider.CurrentUser);
            await chatService.MarkAllMessagesReadAsync(currentUserProvider.CurrentUser, conversation);
            return Ok();
        }

        [HttpPost("{conversationId:long}/read")]
        public async Task<ActionResult<ApiResponse>> MarkConversationAsRead([FromRoute] long conversationId)
        {
            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can read messages from customers", "chats.notAuthorized");
            }

            var conversation = await chatService.GetConversationByIdOrNullAsync(conversationId);
            if (conversation == null)
            {
                return ConversationNotFound();
            }

            await chatService.MarkAllMessagesReadAsync(currentUserProvider.CurrentUser, conversation);
            return Ok();
        }
    }
}
