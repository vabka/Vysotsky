using System;
using System.Collections.Generic;
using Flurl;

namespace Vysotsky.API.Models
{
    public class PaginatedData<T>
    {
        public int Total { get; init; }
        public int PageSize { get; init; }
        public int PageNumber { get; init; }

        public Paginator Iterator { get; } = new();
        public IReadOnlyCollection<T> Data { get; init; } = Array.Empty<T>();
    }

    public static class PaginatedData
    {
        public static PaginatedData<T> Create<T>(PaginationParameters paginationParameters, int total,
            IReadOnlyCollection<T> data,
            string resource)
        {
            var hasNext = total - paginationParameters.ToSkip - data.Count > 0;
            return new PaginatedData<T>
            {
                Total = total,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize,
                Data = data,
                Iterator =
                {
                    Next = hasNext
                        ? resource.SetQueryParams(paginationParameters with
                        {
                            PageNumber = paginationParameters.PageNumber + 1
                        })
                        : null,
                    Previous = paginationParameters.PageNumber > 1
                        ? resource.SetQueryParams(paginationParameters with
                        {
                            PageNumber = paginationParameters.PageNumber - 1
                        })
                        : null
                }
            };
        }
    }
}
