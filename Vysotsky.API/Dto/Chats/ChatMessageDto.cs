using System;

namespace Vysotsky.API.Dto.Chats
{
    public class ChatMessageDto
    {
        public DateTimeOffset CreatedAt { get; init; }
        public ShortPersistedUserDto From { get; init; } = null!;
        public MessageContentDto Content { get; init; } = null!;
        public ChatMessageStatusDto Status { get; init; }
    }
}
