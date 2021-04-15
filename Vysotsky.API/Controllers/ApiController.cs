using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto.Common;


namespace Vysotsky.API.Controllers
{
    /// <summary>
    /// Base API controller
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class ApiController
    {
        protected static ActionResult<ApiResponse<T>> Created<T>(string location, T result) =>
            new CreatedResult(location, CreateSuccess(result));

        protected static ObjectResult NotAuthorized(string message, string code) => Error(message, code, 403);

        protected static ObjectResult Error(string message, string code, int status = 400)
        {
            return new(new ApiResponse
            {
                Error = new ApiError
                {
                    Code = code,
                    Message = message
                },
                Status = ResponseStatus.Error
            })
            {
                StatusCode = status
            };
        }

        /// <summary>
        /// Создать ответ с кодом 200
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static OkObjectResult Ok<T>(T result) => new(CreateSuccess(result));

        protected static OkObjectResult Ok() => new(CreateSuccess());

        /// <summary>
        /// Создать ответ с кодом 400
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected static BadRequestObjectResult BadRequest(string message, string code) =>
            new(new ApiResponse
            {
                Status = ResponseStatus.Error,
                Error = new ApiError
                {
                    Message = message,
                    Code = code
                }
            });

        /// <summary>
        /// Создать ответ с кодом 404
        /// </summary>
        protected NotFoundObjectResult NotFound(string message, string code) =>
            new(new ApiResponse
            {
                Status = ResponseStatus.Error,
                Error = new ApiError
                {
                    Message = message,
                    Code = code,
                }
            });

        private static ApiResponse<T> CreateSuccess<T>(T result) =>
            new()
            {
                Status = ResponseStatus.Ok,
                Result = result
            };

        private static ApiResponse CreateSuccess() =>
            new()
            {
                Status = ResponseStatus.Ok,
            };
    }
}
