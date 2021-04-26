using System;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class ChatMessage
    {
        public long Id { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public long From { get; init; }
        public MessageContent Content { get; init; } = null!;
        public ChatMessageStatus Status { get; init; }
    }
}
