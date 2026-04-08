// <copyright file="PagedResponse.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Paginated envelope. Extends the standard response with pagination metadata.
    /// Data is always a list – never null for paginated results, just empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the response.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Gets an optional message describing the result.
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Gets a collection of error messages, if any.
        /// </summary>
        public IEnumerable<string> Errors { get; init; } = [];

        /// <summary>
        /// Gets the UTC timestamp when the response was created.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the actual page of data.
        /// </summary>
        public List<T> Data { get; init; } = [];

        /// <summary>
        /// Gets the pagination metadata.
        /// </summary>
        public PaginationMeta Pagination { get; init; } = default!;

        /// <summary>
        /// Creates a successful <see cref="PagedResponse{T}"/> instance.
        /// </summary>
        /// <param name="data">The list of items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total count of items.</param>
        /// <param name="message">An optional message describing the result.</param>
        /// <returns>A <see cref="PagedResponse{T}"/> indicating success.</returns>
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

        /// <summary>
        /// Creates a failed <see cref="PagedResponse{T}"/> instance.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <returns>A <see cref="PagedResponse{T}"/> indicating failure.</returns>
        public static PagedResponse<T> Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };
    }

    /// <summary>
    /// Represents pagination metadata.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public record PaginationMeta
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationMeta"/> class.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total count of items.</param>
        public PaginationMeta(int page, int pageSize, int totalCount)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            this.HasPreviousPage = page > 1;
            this.HasNextPage = page < this.TotalPages;
        }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Gets the total count of items.
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; init; }

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage { get; init; }

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage { get; init; }
    }
}
