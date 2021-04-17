using System;

namespace Vysotsky.Service.Types
{
    public record TokenContainer(string token, DateTimeOffset expiresAt, DateTimeOffset issuedAt);
}
