using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Models
{
    public class PagedList<T>
    {
        public PagedList(List<T> data, int pageIndex, int pagesize)
        {
            PageIndex = pageIndex;
            PageSize = pagesize;
            PagedItems = data;


            TotalCount = data.Count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);



        }

        public int PageIndex { get; private set; }


        public int PageSize { get; private set; }


        public int TotalCount { get; private set; }


        public int TotalPages { get; private set; }


        public List<T> PagedItems { get; private set; }

        

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }


        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }


    }
}
