using System;

namespace Vysotsky.API.Dto.Auth
{
    public record AccessTokenContainer(string token, DateTimeOffset expiresAt);
}
