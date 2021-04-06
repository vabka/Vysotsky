using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers
{
    [ApiController]
    public abstract class ApiController : Controller
    {
        protected ActionResult<ApiResponse<T>> Success<T>(T result)
        {
            var apiResponse = new ApiResponse<T>
            {
                Status = ResponseStatus.Ok,
                Result = result
            };
            return Ok(apiResponse);
        }

        protected ActionResult<ApiResponse<T>> MalformedRequest<T>(string message)
        {
            var apiResponse = new ApiResponse<T>
            {
                Status = ResponseStatus.Error,
                Error = new ApiError
                {
                    Message = message
                }
            };
            return BadRequest(apiResponse);
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Result { get; init; }
    }
}
