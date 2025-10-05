using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 20;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
