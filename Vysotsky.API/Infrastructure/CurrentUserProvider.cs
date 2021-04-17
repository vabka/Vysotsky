using System;
using Microsoft.AspNetCore.Http;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Infrastructure
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public User CurrentUser => _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User ??
                                   throw new InvalidOperationException();

    }
}
