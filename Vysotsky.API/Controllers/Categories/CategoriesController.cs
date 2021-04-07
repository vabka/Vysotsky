using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Categories
{
    [Route(Resources.Categories)]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<PaginatedData<Category>>> GetAllCategories([FromQuery] SearchParameters search,
            [FromQuery] PaginationParameters pagination)
        {
            throw new NotImplementedException();
        }
    }
}