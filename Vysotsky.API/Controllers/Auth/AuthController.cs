using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Auth
{
    /// <summary>
    /// Контроллер аутентификации
    /// </summary>
    [Route(Resources.Auth)]
    public class AuthController : ApiController
    {
        [HttpGet("/login")]
        public Task<ActionResult<ApiResponse<AccessTokenContainer>>> Login(
            [FromForm(Name = "username")] string username,
            [FromForm(Name = "password")] string password)
        {
            throw new NotImplementedException();
        }
    }
}
