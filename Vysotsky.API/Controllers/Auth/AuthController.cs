using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service;

namespace Vysotsky.API.Controllers.Auth
{
    /// <summary>
    /// Контроллер аутентификации
    /// </summary>
    [Route(Resources.Auth)]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;

        /// <inheritdoc />
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Выпустить токен
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<ApiResponse<AccessTokenContainer>>> Authenticate(
            [FromBody] Credentials credentials)
        {
            var token = await _authenticationService.TryIssueTokenByUserCredentialsAsync(credentials.Username,
                credentials.Password);
            return token switch
            {
                {Token: var t, ExpiresAt: var exp} => Ok(new AccessTokenContainer(t, exp)),
                _ => BadRequest("Invalid username or password", "auth.invalidUsernameOrPassword")
            };
        }

        /// <summary>
        /// Отозвать текущий токен
        /// </summary>
        /// <returns></returns>
        [HttpPost("unauthenticate")]
        public async Task<ActionResult<ApiResponse>> Unauthenticate([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var authorizationHeaderValue = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];
            if (authorizationHeaderValue.HasValue)
            {
                var tokenText = authorizationHeaderValue.Value.ToString().Substring("Bearer ".Length);
                await _authenticationService.RevokeTokenAsync(tokenText);
            }

            return Ok();
        }
    }
}