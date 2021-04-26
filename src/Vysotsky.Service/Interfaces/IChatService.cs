using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Impl;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessage> SendAsync(User author, Conversation destination, MessageContent messageContent);

        Task<(int Total, IEnumerable<ChatMessage> Data)> GetMessagesAsync(Conversation conversation,
            DateTimeOffset until, Ordering ordering, int skip, int take);

        Task<Conversation?> GetConversationByIdOrNullAsync(long id);
        Task<Conversation> GetConversationByUserAsync(User user);
        Task<IEnumerable<Conversation>> GetAllStartedConversationsAsync();
        Task MarkAllMessagesReadAsync(User reader, Conversation conversation);
    }
}
