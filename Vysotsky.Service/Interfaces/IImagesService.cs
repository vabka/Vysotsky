using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface IImagesService
    {
        public Task<Image?> GetImageByIdOrNullAsync(long id);
    }
}