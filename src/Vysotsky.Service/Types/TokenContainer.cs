using System;

namespace Vysotsky.Service.Types
{
    public record TokenContainer
    {
        public string Token { get; init; } = null!;
        public DateTimeOffset ExpiresAt { get; init; }
        public DateTimeOffset IssuesAt { get; init; }
    }
}
