using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class ChatService : IChatService
    {
        public Task<ChatMessage> SendAsync(User author, Conversation destination, MessageContent messageContent) =>
            throw new NotImplementedException();

        public Task<IEnumerable<ChatMessage>> GetMessagesAsync(Conversation conversation) => throw new NotImplementedException();

        public Task<Conversation?> GetConversationByIdOrNullAsync(long id) => throw new NotImplementedException();

        public Task<Conversation> GetConversationByUserAsync(User user) => throw new NotImplementedException();
        public Task<IEnumerable<Conversation>> GetAllStartedConversations() => throw new NotImplementedException();

        public Task MarkAllMessagesReadAsync(User reader, Conversation conversation) => throw new NotImplementedException();
    }
}
