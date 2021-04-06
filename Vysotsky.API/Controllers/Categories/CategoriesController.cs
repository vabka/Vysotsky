using System;
using System.Linq;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Categories
{
    [Route(Resources.Categories)]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryRepository _categoryRepository;

        [HttpPost]
        public IActionResult CreateCategory()
        {
            throw new NotImplementedException();
        }

        private static readonly Category[] MockData =
        {
            new("A"), new("B"), new("C")
        };

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<PaginatedData<Category>>> GetAllCategories([FromQuery] SearchParameters search,
            [FromQuery] PaginationParameters pagination)
        {
            return Ok(PaginatedData.Create(pagination, 3,
                MockData
                    .Skip(pagination.ToSkip())
                    .Take(pagination.ToTake())
                    .ToArray(),
                string.IsNullOrEmpty(search.Query)
                    ? Resources.Categories
                    : Resources.Categories.SetQueryParam("q", search.Query)));
        }
    }
}
