using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class ChatService : IChatService
    {
        public Task SendAsync(User author, Chat destination, MessageContent messageContent) =>
            throw new NotImplementedException();

        public Task<IEnumerable<ChatMessage>> GetMessagesAsync(Chat chat) => throw new NotImplementedException();

        public Task<Chat?> GetChatByIdOrNullAsync(long id) => throw new NotImplementedException();

        public Task<Chat> GetChatByUserAsync(User user) => throw new NotImplementedException();
        public Task MarkAllMessagesReadAsync(User reader, Chat chat) => throw new NotImplementedException();
    }
}
