using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class CategoryService : ICategoriesService
    {
        private readonly VysotskyDataConnection db;

        public CategoryService(VysotskyDataConnection db) => this.db = db;

        public async Task<Category> CreateAsync(string categoryName, Image image)
        {
            var id = await db.Categories.InsertWithInt64IdentityAsync(() => new CategoryRecord
            {
                Name = categoryName, ImageId = image.Id
            });
            return await GetById(id);
        }

        public async Task<Category> ChangeAsync(Category category, string newName, Image newImage)
        {
            await db.Categories.UpdateAsync(x => x.Id == category.Id,
                c => new CategoryRecord {Name = newName, ImageId = newImage.Id});
            return await GetById(category.Id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync() =>
            await db.Categories
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Id)
                .Select(x => new Category {Id = x.Id, Name = x.Name, ImageId = x.ImageId})
                .ToArrayAsync();

        private async Task<Category> GetById(long id) => await db.Categories
            .Select(x => new Category {Id = x.Id, Name = x.Name, ImageId = x.ImageId})
            .FirstAsync(x => x.Id == id);

        public async Task<Category?> GetByIdOrNullAsync(long categoryId) =>
            await db.Categories.Where(x => x.Id == categoryId)
                .Select(x => new Category {Id = x.Id, Name = x.Name, ImageId = x.ImageId})
                .FirstOrDefaultAsync();

        public Task RemoveAsync(Category category) => throw new System.NotImplementedException();
    }
}
