using System;
using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Dto.Common
{
    public record PaginationParameters
    {
        /// <summary>
        /// Максимальная дата (нужно, чтобы при переключении страниц не возникало дубликатов)
        /// </summary>
        [FromQuery(Name = "until")]
        public DateTimeOffset Until { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Размер страницы, не меньше 1
        /// </summary>
        [FromQuery(Name = "pageSize")]
        public int PageSize { get; init; } = 50;

        /// <summary>
        /// Номер страницы, начиная с 1
        /// </summary>
        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; init; } = 1;

        public int ToSkip() => (PageNumber - 1) * PageSize;
        public int ToTake() => PageSize;
    }
}
