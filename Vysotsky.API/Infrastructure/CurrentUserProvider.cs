using Microsoft.AspNetCore.Http;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Infrastructure
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public User? CurrentUser => httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
    }
}
