using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Flurl;

namespace Vysotsky.API.Dto.Common
{
    /// <summary>
    /// Данные с разбивкой по страницам
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedData<T>
    {
        /// <summary>
        /// Всего записей
        /// </summary>
        public int Total { get; init; }

        /// <summary>
        /// Размер сраницы
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber { get; init; }

        /// <summary>
        /// Дата, до которой предоставляются данные (верхняя граница)
        /// </summary>
        public DateTime Until { get; init; }

        /// <summary>
        /// Страницы для перехода
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pages? Pages { get; init; }

        /// <summary>
        /// Данные
        /// </summary>

        public IReadOnlyCollection<T> Data { get; init; } = Array.Empty<T>();
    }

    internal static class PaginatedData
    {
        public static PaginatedData<T> Create<T>(PaginationParameters paginationParameters, int total,
            IReadOnlyCollection<T> data,
            string resource)
        {
            var hasNext = total - paginationParameters.ToSkip() - data.Count > 0;
            var next = hasNext
                ? resource
                    .SetQueryParams(
                        paginationParameters with { PageNumber = paginationParameters.PageNumber + 1 })
                : null;

            var hasPrevious = paginationParameters.PageNumber > 1;
            var previous = hasPrevious
                ? resource
                    .SetQueryParams(
                        paginationParameters with { PageNumber = paginationParameters.PageNumber - 1 })
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
