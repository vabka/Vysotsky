using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Vysotsky.API.Dto.Auth;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers.Auth
{
    [Route(Resources.Auth)]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService) => this.authenticationService = authenticationService;

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<ApiResponse<AccessTokenContainer>>> Authenticate(
            [FromBody] LoginDto loginDto)
        {
            var token = await this.authenticationService.TryIssueTokenByUserCredentialsAsync(loginDto.Username,
                loginDto.Password);
            return token switch
            {
                { Token: var t, ExpiresAt: var exp } => Ok(new AccessTokenContainer(t, exp)),
                _ => BadRequest("Invalid username or password", "auth.invalidUsernameOrPassword")
            };
        }

        [HttpPost("unauthenticate")]
        public async Task<ActionResult<ApiResponse>> Unauthenticate([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var authorizationHeaderValue = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];
            if (authorizationHeaderValue.HasValue)
            {
                var tokenText = authorizationHeaderValue.Value.ToString().Substring("Bearer ".Length);
                await this.authenticationService.RevokeTokenAsync(tokenText);
            }

            return Ok();
        }
    }
}
