using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Controllers
{
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
            var apiResponse = new ApiResponse<T>()
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

    public class ApiResponse<T>
    {
        public ResponseStatus Status { get; init; }
        public ApiError? Error { get; init; }
        public T? Result { get; init; }
    }

    public enum ResponseStatus
    {
        Ok,
        Error
    }

    public class ApiError
    {
        public string Message { get; init; } = "";
    }

    public class Paginated<T>
    {
        public int Total { get; init; }
        public int Count { get; set; }
        public int PageSize { get; init; }
        public int PageNumber { get; init; }
        public bool HasMore { get; init; }
        public string? NextPage { get; init; }
        public IReadOnlyCollection<T> Data { get; init; }
    }
}
