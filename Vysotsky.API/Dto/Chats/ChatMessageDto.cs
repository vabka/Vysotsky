using System;

namespace Vysotsky.API.Dto.Chats
{
    public class ChatMessageDto
    {
        public DateTimeOffset CreatedAt { get; init; }
        public long From { get; init; }
        public MessageContentDto Content { get; init; } = null!;
        public ChatMessageStatusDto Status { get; init; }
    }
}
