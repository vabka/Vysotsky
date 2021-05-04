using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Common
{
    public class PaginatedData<T> : WrappedListDto<T>
    {
        public PaginationData Pagination { get; init; } = null!;
    }

    public class PaginationData
    {
        public int Total { get; init; }

        public int PageSize { get; init; }

        public int PageNumber { get; init; }

        public DateTimeOffset Until { get; init; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pages? Pages { get; init; }
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
                Pagination = new PaginationData
                {
                    Total = total,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize,
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
                },
                Data = data,
            };
        }
    }
}
