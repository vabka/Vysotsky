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

        public async Task<ChatMessage> SendAsync(User author, Conversation destination, MessageContent messageContent)
        {
            var transaction = await db.BeginTransactionAsync();
            var msgId = await db.Messages.InsertWithInt64IdentityAsync(() => new SupportChatMessageRecord
            {
                AuthorId = author.Id,
                UserId = destination.AttachedUserId,
                Status = ChatMessageStatus.Sent,
                TextContent = messageContent.Text
            });
            if (author.Id == destination.AttachedUserId)
            {
                await db.Conversations.UpdateAsync(x => x.Id == destination.AttachedUserId,
                    x => new ConversationRecord {HasUnreadForSupport = true});
            }
            else
            {
                await db.Conversations.UpdateAsync(x => x.Id == destination.AttachedUserId,
                    x => new ConversationRecord {HasUnreadForCustomer = true});
            }

            await transaction.CommitAsync();
            return await db.Messages
                .Where(x => x.Id == msgId)
                .InnerJoin(db.Users, (l, r) => l.AuthorId == r.Id,
                    (l, r) => new {Message = l, Author = r})
                .Select(m => new ChatMessage
                {
                    Content = new MessageContent {Text = m.Message.TextContent},
                    From = m.Author.Id,
                    Status = m.Message.Status,
                    CreatedAt = m.Message.CreatedAt
                })
                .SingleAsync();
        }

        public async Task<(int Total, IEnumerable<ChatMessage> Data)> GetMessagesAsync(Conversation conversation,
            DateTimeOffset until, int skip, int take)
        {
            var query = db.Messages
                .Where(x => x.UserId == conversation.AttachedUserId)
                .Where(x => x.CreatedAt < until)
                .Select(x => new ChatMessage
                {
                    Content = new MessageContent {Text = x.TextContent,},
                    From = x.AuthorId,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt
                });
            var total = await query.CountAsync();
            var data = await query.Skip(skip).Take(take).ToArrayAsync();
            return (total, data);
        }

        public async Task<Conversation?> GetConversationByIdOrNullAsync(long id) =>
            await db.Conversations
                .Where(x => x.Id == id)
                .Select(x => new Conversation {AttachedUserId = x.Id, HasUnreadMessages = x.HasUnreadForSupport})
                .FirstOrDefaultAsync();

        public async Task<Conversation> GetConversationByUserAsync(User user)
        {
            var conversation = await db.Conversations.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (conversation == null)
            {
                await db.Conversations.InsertAsync(() =>
                    new ConversationRecord {Id = user.Id, HasUnreadForCustomer = false, HasUnreadForSupport = false});
            }

            return new Conversation
            {
                AttachedUserId = user.Id, HasUnreadMessages = conversation?.HasUnreadForSupport ?? false
            };
        }

        public async Task<IEnumerable<Conversation>> GetAllStartedConversationsAsync() =>
            await db.Conversations
                .Select(
                    x => new Conversation {AttachedUserId = x.Id, HasUnreadMessages = x.HasUnreadForSupport})
                .OrderByDescending(x => x.HasUnreadMessages).ToArrayAsync();

        public async Task MarkAllMessagesReadAsync(User reader, Conversation conversation)
        {
            await using var t = await db.BeginTransactionAsync();
            await db.Messages.UpdateAsync(x =>
                    x.UserId == conversation.AttachedUserId &&
                    x.AuthorId != reader.Id &&
                    x.Status == ChatMessageStatus.Sent,
                x => new SupportChatMessageRecord {Status = ChatMessageStatus.Read}
            );
            switch (reader.Role)
            {
                case UserRole.Supervisor or UserRole.SuperUser:
                    await db.Conversations.UpdateAsync(x => x.Id == conversation.AttachedUserId,
                        x => new ConversationRecord {HasUnreadForSupport = false});
                    break;
                case UserRole.OrganizationMember or UserRole.OrganizationOwner:
                    await db.Conversations.UpdateAsync(x => x.Id == conversation.AttachedUserId,
                        x => new ConversationRecord {HasUnreadForCustomer = false});
                    break;
                default:
                    throw new InvalidOperationException();
            }

            await t.CommitAsync();
        }
    }
}
