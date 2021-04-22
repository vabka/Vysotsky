using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class CategoryService : ICategoriesService
    {
        private readonly VysotskyDataConnection db;

        public CategoryService(VysotskyDataConnection db) => this.db = db;

        public Task<Category> CreateAsync(string categoryName, Image image) =>
            throw new System.NotImplementedException();

        public async Task<IEnumerable<Category>> GetAllAsync() =>
            await db.Categories
                .OrderBy(x => x.Id)
                .Select(x => new Category {Id = x.Id, Name = x.Name, ImageId = x.ImageId})
                .ToArrayAsync();

        public async Task<Category?> GetByIdOrNullAsync(long categoryId) =>
            await db.Categories.Where(x => x.Id == categoryId)
                .Select(x => new Category {Id = x.Id, Name = x.Name, ImageId = x.ImageId})
                .FirstOrDefaultAsync();

        public Task RemoveAsync(Category category) => throw new System.NotImplementedException();
    }
}
