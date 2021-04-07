﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vysotsky.API.Controllers.Common;

namespace Vysotsky.API.Infrastructure
{
    internal class UnhandledExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<UnhandledExceptionMiddleware> _logger;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                if (!context.Response.HasStarted)
                    context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Status = ResponseStatus.Error,
                    Error = new ApiError
                    {
                        Message = "Unexpected error",
                        Code = "internalError"
                    }
                });
                _logger.LogError(exception, "Unhandled exception");
            }
        }
    }
}
