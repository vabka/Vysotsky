using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Infrastructure
{
    public interface ICurrentUserProvider
    {
        User? CurrentUser { get; }
    }
}
