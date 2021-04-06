using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers
{
    [ApiController]
    public abstract class ApiController
    {
        protected ActionResult<ApiResponse<T>> Created<T>(string location, T result)
        {
            var apiResponse = CreateSuccess(result);
            return new CreatedResult(location, apiResponse);
        }

        protected ActionResult<ApiResponse<T>> Ok<T>(T result)
        {
            var apiResponse = CreateSuccess(result);
            return new OkObjectResult(apiResponse);
        }

        protected ActionResult<ApiResponse<T>> BadRequest<T>(string message)
        {
            var apiResponse = new ApiResponse<T>
            {
                Status = ResponseStatus.Error,
                Error = new ApiError
                {
                    Message = message
                }
            };
            return new BadRequestObjectResult(apiResponse);
        }

        private static ApiResponse<T> CreateSuccess<T>(T result)
        {
            var apiResponse = new ApiResponse<T>
            {
                Status = ResponseStatus.Ok,
                Result = result
            };
            return apiResponse;
        }
    }
}
