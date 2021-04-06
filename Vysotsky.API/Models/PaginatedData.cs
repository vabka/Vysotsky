using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Flurl;

namespace Vysotsky.API.Models
{
    public class PaginatedData<T>
    {
        public int Total { get; init; }
        public int PageSize { get; init; }
        public int PageNumber { get; init; }
        public DateTime Until { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pages? Pages { get; init; }

        public IReadOnlyCollection<T> Data { get; init; } = Array.Empty<T>();
    }

    public static class PaginatedData
    {
        public static PaginatedData<T> Create<T>(PaginationParameters paginationParameters, int total,
            IReadOnlyCollection<T> data,
            string resource)
        {
            var hasNext = total - paginationParameters.ToSkip() - data.Count > 0;
            var next = hasNext
                ? resource
                    .SetQueryParams(
                        paginationParameters with {PageNumber = paginationParameters.PageNumber + 1})
                : null;

            var hasPrevious = paginationParameters.PageNumber > 1;
            var previous = hasPrevious
                ? resource
                    .SetQueryParams(
                        paginationParameters with {PageNumber = paginationParameters.PageNumber - 1})
                : null;
            
            return new PaginatedData<T>
            {
                Total = total,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize,
                Data = data,
                Until = paginationParameters.Until,
                Pages = hasNext || hasPrevious
                    ? new Pages
                    {
                        Next = next,
                        Previous = previous
                    }
                    : null
            };
        }
    }
}
