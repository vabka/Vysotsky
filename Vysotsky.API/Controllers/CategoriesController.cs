using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers
{
    [Route("/api/categories")]
    public class CategoriesController : ApiController
    {
        [HttpPost]
        public IActionResult CreateCategory()
        {
            return Ok();
        }

        [HttpGet]
        public ActionResult<ApiResponse<PaginatedData<Category>>> GetAllCategories(
            [FromQuery(Name = "q")] string? query,
            [FromQuery(Name = "page_size")] int? pageSize,
            [FromQuery(Name = "page_number")] int? pageNumber)
        {
            pageNumber ??= 1;
            pageSize ??= 50;
            var result = new PaginatedData<Category>
            {
                Total = 2,
                Count = 2,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value,
                Iterator =
                {
                    Previous = null,
                    Next = "/categories".SetQueryParams(new
                    {
                        query,
                        pageSize,
                        pageNumber,
                    })
                },

                Data =
                {
                    new()
                    {
                        Name = "Электрика"
                    },
                    new()
                    {
                        Name = "Вентиляция"
                    },
                },
            };
            return Success(result);
        }
    }
}
