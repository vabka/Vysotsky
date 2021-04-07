using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Categories
{
    /// <summary>
    /// Контроллер категорий заявок
    /// </summary>
    [Route(Resources.Categories)]
    public class CategoriesController : ApiController
    {
        /// <summary>
        /// Получить все категории работ
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        public ActionResult<ApiResponse<PaginatedData<Category>>> GetAllCategories([FromQuery] SearchParameters search,
            [FromQuery] PaginationParameters pagination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить все подкатегории работ
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pagination"></param>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<PaginatedData<SubCategory>>> GetAllSubcategories(
            [FromQuery] SearchParameters search,
            [FromQuery] PaginationParameters pagination,
            [FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}