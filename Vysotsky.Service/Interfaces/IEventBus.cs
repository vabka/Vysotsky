using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IEventBus
    {
        Task PushAsync<TEvent>(TEvent data) where TEvent : Event;
    }
}