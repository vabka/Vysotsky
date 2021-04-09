using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Users
{
    [Route(Resources.Users)]
    public class UsersController : ApiController
    {
        [HttpPost]
        public ActionResult<ApiResponse> RegisterUser(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}