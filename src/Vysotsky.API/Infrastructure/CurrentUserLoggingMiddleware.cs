using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Vysotsky.API.Infrastructure
{
    public class CurrentUserLoggingMiddleware : IMiddleware
    {
        private readonly ICurrentUserProvider currentUserProvider;

        public CurrentUserLoggingMiddleware(ICurrentUserProvider currentUserProvider) =>
            this.currentUserProvider = currentUserProvider;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (currentUserProvider.IsAuthenticated)
            {
                LogContext.PushProperty("CurrentUserName", currentUserProvider.CurrentUser.Username);
            }

            await next(context);
        }
    }
}
