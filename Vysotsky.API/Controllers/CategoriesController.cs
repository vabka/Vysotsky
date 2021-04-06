using System;
using System.Linq;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Categories)]
    public class CategoriesController : ApiController
    {
        [HttpPost]
        public IActionResult CreateCategory()
        {
            throw new NotImplementedException();
        }

        private static readonly Category[] MockData =
        {
            new() {Name = "A"}, new() {Name = "B"}, new() {Name = "C"}
        };

        [HttpGet]
        public ActionResult<ApiResponse<PaginatedData<Category>>> GetAllCategories([FromQuery] SearchParameters search,
            [FromQuery] PaginationParameters pagination)
        {
            return Ok(PaginatedData.Create(pagination, 3,
                MockData
                    .Skip(pagination.ToSkip)
                    .Take(pagination.ToTake)
                    .ToArray(),
                Resources.Categories));
        }
    }
}
