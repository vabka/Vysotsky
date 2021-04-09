using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Vysotsky.API.Controllers.Common;
using Vysotsky.Data;

namespace Vysotsky.API.Infrastructure
{
    public class BlockedTokenMiddleware : IMiddleware
    {
        private readonly VysotskyDataConnection _dataConnection;

        public BlockedTokenMiddleware(VysotskyDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                var header = context.Request.Headers[HeaderNames.Authorization];
                if (header[0].StartsWith("Bearer "))
                {
                    var token = header[0].Substring("Bearer ".Length);
                    var blocked = await _dataConnection.BlockedTokens.AnyAsync(x => x.Value == token);
                    if (blocked)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new ApiResponse
                        {
                            Status = ResponseStatus.Error,
                            Error = new ApiError
                            {
                                Code = "auth.tokenRevoked",
                                Message = "This access token is blocked"
                            }
                        });
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}