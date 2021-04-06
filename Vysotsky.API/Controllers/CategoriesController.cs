using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Controllers
{
    [Route("/api/categories")]
    public class CategoriesController : ApiController
    {
        [HttpPost]
        public IActionResult CreateCategory()
        {
            return NoContent();
        }

        [HttpGet]
        public ActionResult<ApiResponse<Paginated<Category>>> GetAllCategories([FromQuery(Name = "q")] string? query,
            [FromQuery(Name = "page_size")] int? pageSize,
            [FromQuery(Name = "page_number")] int? pageNumber)
        {
            var result = new Paginated<Category>
            {
                Total = 2,
                Count = 2,
                PageNumber = pageNumber ?? 0,
                PageSize = pageSize ?? 50,
                HasMore = false,
                NextPage = null,
                Data = new Category[]
                {
                    new()
                    {
                        Name ="Электрика" 
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

    public class Category
    {
        public string Name { get; init; } = "";
    }
}
