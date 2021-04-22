using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwag.Annotations;

namespace Vysotsky.API.Dto.Common
{
    public class HackedDateTimeOffsetBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName).FirstValue;
            bindingContext.Result = value switch
            {
                ""                                                    => ModelBindingResult.Success(DateTimeOffset.Now),
                { } str when DateTimeOffset.TryParse(str, out var dt) => ModelBindingResult.Success(dt),
                _                                                     => bindingContext.Result
            };

            return Task.CompletedTask;
        }
    }

    public record PaginationParameters
    {
        [FromQuery(Name = "until")]
        [BindingBehavior(BindingBehavior.Optional)]
        [ModelBinder(typeof(HackedDateTimeOffsetBinder))]
        public DateTimeOffset Until { get; init; } = DateTimeOffset.Now;

        [FromQuery(Name = "pageSize")] public int PageSize { get; init; } = 50;

        [FromQuery(Name = "pageNumber")] public int PageNumber { get; init; }

        [OpenApiIgnore]
        [BindNever]
        [JsonIgnore]
        public int Skip => PageNumber * PageSize;

        [OpenApiIgnore]
        [BindNever]
        [JsonIgnore]
        public int Take => PageSize;
    }
}
