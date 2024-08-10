using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.Responses
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } // The list of items for the current page
        public int TotalItems { get; set; } // The total number of items across all pages
        public int PageNumber { get; set; } // The current page number
        public int PageSize { get; set; } // The size of each page
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize); // The total number of pages

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(List<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
