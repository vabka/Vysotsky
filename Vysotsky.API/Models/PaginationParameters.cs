using System;
using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Models
{
    public record PaginationParameters
    {
        [FromQuery(Name = "until")]
        public DateTime Until { get; init; } = DateTime.Now;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; init; } = 50;

        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; init; } = 1;

        public int ToSkip() => (PageNumber - 1) * PageSize;
        public int ToTake() => PageSize;
    }
}
