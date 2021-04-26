using System;

namespace Vysotsky.API.Dto.Auth
{
    public record AccessTokenDto
    {
        public string Token { get; init; } = null!;
        public DateTimeOffset ExpiresAt { get; init; }
    }
}
