// <copyright file="PagedQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Base for all paginated queries.
    /// Enforces consistent page/pageSize parameters across the system.
    /// </summary>
    public abstract record PagedQuery
    {
        private int page = 1;
        private int pageSize = 20;

        /// <summary>
        /// Gets the current page number. Defaults to 1 if value is less than 1.
        /// </summary>
        public int Page
        {
            get => this.page;
            init => this.page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Gets the page size. Defaults to 20 if value is less than 1, capped at 100.
        /// </summary>
        public int PageSize
        {
            get => this.pageSize;
            init => this.pageSize = value switch
            {
                < 1 => 20,
                > 100 => 100,
                _ => value,
            };
        }

        /// <summary>
        /// Gets the number of items to skip based on the current page and page size.
        /// </summary>
        public int Skip => (this.Page - 1) * this.PageSize;
    }
}
