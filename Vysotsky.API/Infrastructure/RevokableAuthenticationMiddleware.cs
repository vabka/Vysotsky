using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Infrastructure
{
    public class RevokableAuthenticationMiddleware : IMiddleware
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public RevokableAuthenticationMiddleware(IAuthenticationService authenticationService,
            IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity is ClaimsIdentity { IsAuthenticated: true } identity)
            {
                var claims = identity.Claims.ToDictionary(x => x.Type, x => x.Value);
                var jti = Guid.Parse(claims["jti"]);
                var iat = long.Parse(claims["iat"], CultureInfo.InvariantCulture);
                // Должен быть sub, но asp net почему-то ставит это.
                var sub = claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
                if (await _authenticationService.CheckTokenRevokedAsync(jti))
                {
                    throw new SecurityTokenRevokedException();
                }

                var lastPasswordChange = await _authenticationService.TryGetLastPasswordChangeTimeAsync(sub);
                if (lastPasswordChange == null || lastPasswordChange.Value.ToUnixTimeSeconds() >= iat)
                {
                    await _authenticationService.RevokeTokenByJtiAsync(jti,
                        DateTimeOffset.FromUnixTimeSeconds(iat));
                    throw new SecurityTokenIsNotActualException();
                }

                var user = await _userService.GetUserByUsernameOrNullAsync(sub);
                context.Items.Add("CurrentUser", user);
            }

            await next(context);
        }
    }
}
