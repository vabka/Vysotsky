using System.Threading.Tasks;

namespace Vysotsky.API.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(long chatId, long author, string text);
    }
}