using CommonObjects.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.HelperServices
{
    public class PaginationHelper
    {
        /// <summary>
        /// Paginate a list of items IN MEMORY 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedResponse<T> Paginate<T>(List<T> source, int pageNumber, int pageSize) where T : class
        {
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 20;
            }
            const int _maxPageSize = 100;
            pageSize = (pageSize > _maxPageSize) ? _maxPageSize : pageSize;
            var pagedResponse = new PagedResponse<T>
            {
                CurrentPageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecordCount = source.Count,
                PageCount = (int)Math.Ceiling(source.Count / (double)pageSize),
                Result = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            };
            return pagedResponse;
        }
    }
}
