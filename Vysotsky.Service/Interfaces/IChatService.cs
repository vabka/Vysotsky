using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IChatService
    {
        Task SendAsync(User author, Chat destination, MessageContent messageContent);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(Chat chat);
        Task<Chat?> GetChatByIdOrNullAsync(long id);
        Task<Chat> GetChatByUserAsync(User user);
    }
}
