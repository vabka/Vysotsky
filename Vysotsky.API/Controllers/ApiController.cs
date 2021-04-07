using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers
{
    /// <summary>
    /// Base API controller
    /// </summary>
    [ApiController]
    public abstract class ApiController
    {
        /// <summary>
        /// Создать отве с кодом 201
        /// </summary>
        /// <param name="location"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ActionResult<ApiResponse<T>> Created<T>(string location, T result)
        {
            var apiResponse = CreateSuccess(result);
            return new CreatedResult(location, apiResponse);
        }

        /// <summary>
        /// Создать ответ с кодом 200
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ActionResult<ApiResponse<T>> Ok<T>(T result)
        {
            var apiResponse = CreateSuccess(result);
            return new OkObjectResult(apiResponse);
        }

        /// <summary>
        /// Создать ответ с кодом 400
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected BadRequestObjectResult BadRequest(string message, string code)
        {
            var apiResponse = new ApiResponse
            {
                Status = ResponseStatus.Error,
                Error = new ApiError
                {
                    Message = message,
                    Code = code
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
