using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto.Categories;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Images;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers.Categories
{
    [Route(Resources.Areas)]
    public class AreasController : ApiController
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IImagesService _imagesService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public AreasController(ICategoriesService categoriesService, IImagesService imagesService,
            ICurrentUserProvider currentUserProvider)
        {
            _categoriesService = categoriesService;
            _imagesService = imagesService;
            _currentUserProvider = currentUserProvider;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedAreaDto>>> CreateArea([FromBody] AreaDto areaDto)
        {
            if (!_currentUserProvider.CanEditCategories())
                return NotAuthorizedToEdit();
            var image = await _imagesService.GetImageByIdOrNullAsync(areaDto.ImageId);
            if (image == null)
                return BadRequest("Image not found", "images.notFound");
            var area = await _categoriesService.CreateAreaAsync(areaDto.Name, image);
            return Created(Resources.Areas.AppendPathSegment(area.Id), new PersistedAreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Image = new PersistedImageDto
                {
                    Id = area.Image.Id,
                }
            });
        }

        private static ObjectResult NotAuthorizedToEdit() =>
            NotAuthorized("Only supervisor can edit categories", "categories.notAuthorized");

        [HttpPost("{areaId:long}/categories")]
        public async Task<ActionResult<ApiResponse<PersistedCategoryDto>>> CreateCategory([FromRoute] long areaId,
            [FromBody] CategoryDto categoryDto)
        {
            if (!_currentUserProvider.CanEditCategories())
                return NotAuthorizedToEdit();
            var area = await _categoriesService.GetAreaByIdOrNullAsync(areaId);
            if (area == null)
                return AreaNotFound();
            var category = await _categoriesService.CreateCategoryInAreaAsync(area, categoryDto.Name);
            return Created(Resources.Areas.AppendPathSegments(areaId, "categories", category.Id),
                new PersistedCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                });
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedAreaDto>>>> GetAllAreas()
        {
            var areas = await _categoriesService.GetAllAreasAsync();
            return Ok(areas.Select(area => new PersistedAreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Image = new PersistedImageDto
                {
                    Id = area.Image.Id
                }
            }));
        }

        [HttpGet("{areaId}/categories")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedCategoryDto>>>> GetAllCategoriesInArea(
            long areaId)
        {
            var area = await _categoriesService.GetAreaByIdOrNullAsync(areaId);
            if (area == null)
                return AreaNotFound();
            var categories = await _categoriesService.GetAllCategoriesInAreaAsync(area);
            return Ok(categories.Select(c => new PersistedCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }));
        }

        private NotFoundObjectResult AreaNotFound() => NotFound("Area not found", "categories.areaNotFound");
    }
}