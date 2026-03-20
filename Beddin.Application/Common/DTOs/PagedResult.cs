
namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// What query handlers return.
    /// Controllers then wrap this in PagedResponse<T>.
    /// Keeps handlers unaware of HTTP concerns.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }

        public static PagedResult<T> From(
            List<T> items,
            int totalCount,
            int page,
            int pageSize) =>
            new()
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
    }
}
