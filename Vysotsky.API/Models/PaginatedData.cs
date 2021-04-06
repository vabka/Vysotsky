using System.Collections.Generic;

namespace Vysotsky.API.Models
{
    public class PaginatedData<T>
    {
        public int Total { get; init; }
        public int Count { get; set; }
        public int PageSize { get; init; }
        public int PageNumber { get; init; }

        public PaginationIterator Iterator { get; } = new();
        public List<T> Data { get; init; } = new();
    }

    public class PaginationIterator
    {
        public string? Previous { get; set; }
        public string? Next { get; set; }
    }
}
