using Microsoft.AspNetCore.Mvc;

namespace Vysotsky.API.Models
{
    public record PaginationParameters([FromQuery(Name = "ts")] long TimeStamp,
        [FromQuery(Name = "perPage")] int PageSize,
        [FromQuery(Name = "page")] int PageNumber)
    {
        public int ToSkip => (PageNumber - 1) * PageSize;
        public int ToTake => PageSize;
    }
}
