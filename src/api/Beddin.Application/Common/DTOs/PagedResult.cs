// <copyright file="PagedResult.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// What query handlers return.
    /// Controllers then wrap this in PagedResponse&lt;T&gt;.
    /// Keeps handlers unaware of HTTP concerns.
    /// </summary>
    /// <typeparam name="T">The type of items in the result.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets the list of items in the current page.
        /// </summary>
        public List<T> Items { get; init; } = [];

        /// <summary>
        /// Gets the total count of items across all pages.
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/> instance from the provided values.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="totalCount">The total count of items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A new <see cref="PagedResult{T}"/> instance.</returns>
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
