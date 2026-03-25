using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Paginated envelope. Extends the standard response with pagination metadata.
    /// Data is always a list � never null for paginated results, just empty.
    /// </summary>
    public class PagedResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public IEnumerable<string> Errors { get; init; } = [];
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        // The actual page of data
        public List<T> Data { get; init; } = [];

        // Pagination metadata
        public PaginationMeta Pagination { get; init; } = default!;

        public static PagedResponse<T> Ok(
            List<T> data,
            int page,
            int pageSize,
            int totalCount,
            string? message = null) =>
            new()
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = new PaginationMeta(page, pageSize, totalCount),
            };

        public static PagedResponse<T> Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };
    }

    public record PaginationMeta
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages { get; init; }
        public bool HasPreviousPage { get; init; }
        public bool HasNextPage { get; init; }

        public PaginationMeta(int page, int pageSize, int totalCount)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            HasPreviousPage = page > 1;
            HasNextPage = page < TotalPages;
        }
    }
}
