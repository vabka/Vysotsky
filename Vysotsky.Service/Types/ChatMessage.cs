using System;
using Vysotsky.Data.Entities;

namespace Vysotsky.Service.Types
{
    public class ChatMessage
    {
        public DateTimeOffset CreatedAt { get; init; }
        public long From { get; init; }
        public MessageContent Content { get; init; } = null!;
        public ChatMessageStatus Status { get; init; }
    }

    public class ShortUser
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string Firstname { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Patronymic { get; init; }
        public UserRole Role { get; init; }
        public long? OrganizationId { get; init; }
    }
}
