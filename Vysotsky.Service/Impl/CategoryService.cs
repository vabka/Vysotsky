using System.Collections.Generic;
using System.Threading.Tasks;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.Service.Impl
{
    public class CategoryService:ICategoriesService
    {
        public Task<Area> CreateAreaAsync(string areaName, Image image) => throw new System.NotImplementedException();

        public Task<Area?> GetAreaByIdOrNullAsync(long areaId) => throw new System.NotImplementedException();

        public Task<Category> CreateCategoryInAreaAsync(Area area, string categoryDtoName) => throw new System.NotImplementedException();

        public Task<IEnumerable<Area>> GetAllAreasAsync() => throw new System.NotImplementedException();

        public Task<IEnumerable<Category>> GetAllCategoriesInAreaAsync(Area area) => throw new System.NotImplementedException();

        public Task<Category?> GetCategoryByIdOrNullAsync(long dataCategoryId) => throw new System.NotImplementedException();
    }
}
