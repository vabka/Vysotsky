using System;

namespace Vysotsky.API.Controllers.Auth
{
    public record AccessTokenContainer(string Token, DateTimeOffset ExpiresAt);
}