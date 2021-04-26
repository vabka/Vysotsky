using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface ICategoriesService
    {
        Task<Category> CreateAsync(string categoryName, Image image);
        Task<Category> ChangeAsync(Category category, string newName, Image newImage);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdOrNullAsync(long categoryId);
        Task RemoveAsync(Category category);
    }
}
