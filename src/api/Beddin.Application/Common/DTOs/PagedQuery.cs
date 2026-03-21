using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Base for all paginated queries.
    /// Enforces consistent page/pageSize parameters across the system.
    /// </summary>
    public abstract record PagedQuery
    {
        private int _page = 1;
        private int _pageSize = 20;

        public int Page
        {
            get => _page;
            init => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value switch
            {
                < 1 => 20,
                > 100 => 100,   // hard cap — never allow unbounded queries
                _ => value
            };
        }

        public int Skip => (Page - 1) * PageSize;
    }
}
