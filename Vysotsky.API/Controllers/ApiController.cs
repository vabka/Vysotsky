using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;


namespace Vysotsky.API.Controllers
{
    /// <summary>
    /// Base API controller
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class ApiController
    {
        /// <summary>
        /// Создать ответ с кодом 201
        /// </summary>
        /// <param name="location"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ActionResult<ApiResponse<T>> Created<T>(string location, T result) =>
            new CreatedResult(location, CreateSuccess(result));

        /// <summary>
        /// Создать ответ с произвольным кодом, без контента
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected static ActionResult Code(int code) => new StatusCodeResult(code);

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