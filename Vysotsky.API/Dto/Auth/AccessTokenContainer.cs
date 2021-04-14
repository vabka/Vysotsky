using System;

namespace Vysotsky.API.Dto.Auth
{
    public record AccessTokenContainer(string Token, DateTimeOffset ExpiresAt);
}