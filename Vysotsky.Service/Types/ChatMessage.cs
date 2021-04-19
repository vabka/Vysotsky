using System;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Types
{
    public class ChatMessage
    {
        public DateTimeOffset CreatedAt { get; init; }
        public User From { get; init; } = null!;
        public MessageContent Content { get; init; } = null!;
    }
}