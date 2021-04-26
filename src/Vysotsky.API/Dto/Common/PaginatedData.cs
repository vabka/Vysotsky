using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Common
{
    /// <summary>
    /// Данные с разбивкой по страницам
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedData<T>
    {
        public int Total { get; init; }

        public int PageSize { get; init; }

        public int PageNumber { get; init; }

        public DateTimeOffset Until { get; init; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pages? Pages { get; init; }


        public IEnumerable<T> Data { get; init; } = Array.Empty<T>();
    }

    internal static class PaginatedData
    {
        public static PaginatedData<T> Create<T>(PaginationParameters paginationParameters, int total,
            IEnumerable<T> lines)
        {
            var data = lines.ToArray();
            var hasNext = total - paginationParameters.Skip - data.Length > 0;
            var hasPrevious = paginationParameters.PageNumber > 0;

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
                        Next =
                            hasNext
                                ? paginationParameters with {PageNumber = paginationParameters.PageNumber + 1,}
                                : null,
                        Previous = hasPrevious
                            ? paginationParameters with {PageNumber = paginationParameters.PageNumber - 1,}
                            : null
                    }
                    : null
            };
        }
    }
}
