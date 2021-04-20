using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class CategoryService : ICategoriesService
    {
        public Task<Category> CreateAsync(string categoryName, Image image) => throw new System.NotImplementedException();
        public Task<IEnumerable<Category>> GetAllAsync() => throw new System.NotImplementedException();
        public Task<Category?> GetByIdOrNullAsync(long categoryId) => throw new System.NotImplementedException();
        public Task RemoveAsync(Category category) => throw new System.NotImplementedException();
    }
}
