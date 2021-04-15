using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Vysotsky.API.Dto.Common;

namespace Vysotsky.API.Infrastructure
{
    internal class UnhandledExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<UnhandledExceptionMiddleware> logger;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger) =>
            this.logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (SecurityTokenException invalidTokenException)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                }

                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Status = ResponseStatus.Error,
                    Error = new ApiError
                    {
                        Message = "Invalid access token",
                        Code = "auth.invalidAccessToken"
                    }
                });
                this.logger.LogInformation(invalidTokenException, "Invalid token");
            }
            catch (Exception exception)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                }

                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Status = ResponseStatus.Error,
                    Error = new ApiError
                    {
                        Message = "Unexpected error",
                        Code = "internalError"
                    }
                });
                this.logger.LogError(exception, "Unhandled exception");
            }
        }
    }
}
