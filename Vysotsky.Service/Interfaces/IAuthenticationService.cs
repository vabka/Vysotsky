using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenContainer?> TryIssueTokenByUserCredentialsAsync(string username, string password,
            bool longLiving = false);

        Task RevokeTokenAsync(string token);
    }
}