using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Interfaces
{
    public interface ICategoriesService
    {
        Task<Area> CreateAreaAsync(string areaName, Image image);
        Task<Area?> GetAreaByIdOrNullAsync(long areaId);
        Task<Category> CreateCategoryInAreaAsync(Area area, string categoryDtoName);
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<IEnumerable<Category>> GetAllCategoriesInAreaAsync(Area area);
        Task<Category?> GetCategoryByIdOrNullAsync(long dataCategoryId);
    }
}
