using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class ChatService : IChatService
    {
        private readonly VysotskyDataConnection db;

        public ChatService(VysotskyDataConnection vysotskyDataConnection) =>
            db = vysotskyDataConnection;

        public Task<ChatMessage> SendAsync(User author, Conversation destination, MessageContent messageContent) =>
            throw new NotImplementedException();

        public Task<IEnumerable<ChatMessage>> GetMessagesAsync(Conversation conversation) =>
            throw new NotImplementedException();

        public async Task<Conversation?> GetConversationByIdOrNullAsync(long id) =>
            await db.Conversations
                .Where(x => x.Id == id)
                .Select(x => new Conversation {AttachedUserId = x.Id, HasUnreadMessages = x.HasUnread})
                .SingleOrDefaultAsync();

        public async Task<Conversation> GetConversationByUserAsync(User user)
        {
            var conversation = await db.Conversations.SingleOrDefaultAsync(x => x.Id == user.Id);
            if (conversation == null)
            {
                await db.Conversations.InsertAsync(() => new ConversationRecord {Id = user.Id, HasUnread = false});
            }

            return new Conversation {AttachedUserId = user.Id, HasUnreadMessages = conversation?.HasUnread ?? false};
        }

        public async Task<IEnumerable<Conversation>> GetAllStartedConversations()
        {
            var chats = await db.Conversations.OrderByDescending(x => x.HasUnread).ToArrayAsync();
            return chats.Select(x => new Conversation {AttachedUserId = x.Id, HasUnreadMessages = x.HasUnread});
        }

        public async Task MarkAllMessagesReadAsync(User reader, Conversation conversation)
        {
            await using var t = await db.BeginTransactionAsync();
            await db.Messages.UpdateAsync(x =>
                    x.UserId == conversation.AttachedUserId &&
                    x.AuthorId != reader.Id &&
                    x.Status == ChatMessageStatus.Sent,
                x => new SupportChatMessageRecord {Status = ChatMessageStatus.Read}
            );
            await db.Conversations.UpdateAsync(x => x.Id == conversation.AttachedUserId,
                x => new ConversationRecord {HasUnread = false});
            await t.CommitAsync();
        }
    }
}
