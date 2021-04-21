using System;
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
    [Route(Resources.Categories)]
    public class CategoriesController : ApiController
    {
        private readonly ICategoriesService categoriesService;
        private readonly IImagesService imagesService;
        private readonly ICurrentUserProvider currentUserProvider;

        public CategoriesController(ICategoriesService categoriesService, IImagesService imagesService,
            ICurrentUserProvider currentUserProvider)
        {
            this.categoriesService = categoriesService;
            this.imagesService = imagesService;
            this.currentUserProvider = currentUserProvider;
        }

        private static ObjectResult NotAuthorizedToEdit() =>
            NotAuthorized("Only supervisor can edit categories", "categories.notAuthorized");

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedCategoryDto>>> CreateCategory(
            [FromBody] CategoryDto data)
        {
            if (!currentUserProvider.CurrentUser.CanEditCategories())
            {
                return NotAuthorizedToEdit();
            }

            var image = await imagesService.GetImageByIdOrNullAsync(data.ImageId);
            if (image == null)
            {
                throw new InvalidOperationException("TODO");
            }

            var category = await categoriesService.CreateAsync(data.Name, image);
            return Created(Resources.Categories.AppendPathSegments(category.Id), category.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersistedCategoryDto>>>> GetAllCategoriesInArea()
        {
            var categories = await categoriesService.GetAllAsync();
            return Ok(categories.Select(c => c.ToDto()));
        }
    }
}
