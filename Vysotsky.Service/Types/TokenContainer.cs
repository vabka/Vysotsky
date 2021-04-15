using System;

namespace Vysotsky.Service.Types
{
    public record TokenContainer(string Token, DateTimeOffset ExpiresAt, DateTimeOffset IssuedAt);
}
