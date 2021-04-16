using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Categories;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Areas)]
    public class AreasController : ApiController
    {
        private readonly ICategoriesService categoriesService;
        private readonly IImagesService imagesService;
        private readonly ICurrentUserProvider currentUserProvider;

        public AreasController(ICategoriesService categoriesService, IImagesService imagesService,
            ICurrentUserProvider currentUserProvider)
        {
            this.categoriesService = categoriesService;
            this.imagesService = imagesService;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedAreaDto>>> CreateArea([FromBody] AreaDto areaDto)
        {
            if (!currentUserProvider.CanEditCategories())
            {
                return NotAuthorizedToEdit();
            }

            var image = await imagesService.GetImageByIdOrNullAsync(areaDto.ImageId);
            if (image == null)
            {
                return BadRequest("Image not found", "images.notFound");
            }

            var area = await categoriesService.CreateAreaAsync(areaDto.Name, image);
            return Created(Resources.Areas.AppendPathSegment(area.Id), area.ToDto());
        }

        private static ObjectResult NotAuthorizedToEdit() =>
            NotAuthorized("Only supervisor can edit categories", "categories.notAuthorized");

        [HttpPost("{areaId:long}/categories")]
        public async Task<ActionResult<ApiResponse<PersistedCategoryDto>>> CreateCategory([FromRoute] long areaId,
            [FromBody] CategoryDto categoryDto)
        {
            if (!currentUserProvider.CanEditCategories())
            {
                return NotAuthorizedToEdit();
            }

            var area = await categoriesService.GetAreaByIdOrNullAsync(areaId);
            if (area == null)
            {
                return AreaNotFound();
            }

            var category = await categoriesService.CreateCategoryInAreaAsync(area, categoryDto.Name);
            return Created(Resources.Areas.AppendPathSegments(areaId, "categories", category.Id), category.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedAreaDto>>>> GetAllAreas()
        {
            var areas = await categoriesService.GetAllAreasAsync();
            return Ok(areas.Select(area => area.ToDto()));
        }

        [HttpGet("{areaId}/categories")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedCategoryDto>>>> GetAllCategoriesInArea(
            long areaId)
        {
            var area = await categoriesService.GetAreaByIdOrNullAsync(areaId);
            if (area == null)
            {
                return AreaNotFound();
            }

            var categories = await categoriesService.GetAllCategoriesInAreaAsync(area);
            return Ok(categories.Select(c => c.ToDto()));
        }

        private NotFoundObjectResult AreaNotFound() => NotFound("Area not found", "categories.areaNotFound");
    }
}
