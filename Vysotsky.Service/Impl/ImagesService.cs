using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class ImagesService : IImagesService
    {
        private readonly VysotskyDataConnection db;

        public ImagesService(VysotskyDataConnection db) => this.db = db;

        public async Task<Image?> GetImageByIdOrNullAsync(long id) =>
            await db.Images
                .Where(x => x.Id == id)
                .Select(x => new Image {Id = x.Id}).FirstOrDefaultAsync();
    }
}
