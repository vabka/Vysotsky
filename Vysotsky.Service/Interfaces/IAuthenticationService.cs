using System;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenContainer?> TryIssueTokenByUserCredentialsAsync(string username, string password,
            bool longLiving = false);

        Task RevokeTokenAsync(string token);

        Task RevokeTokenByJtiAsync(Guid tokenIdentifier, DateTimeOffset expiration);
        Task<DateTimeOffset?> TryGetLastPasswordChangeTimeAsync(string username);
        Task<bool> CheckTokenRevokedAsync(Guid tokenIdentifier);
    }
}
