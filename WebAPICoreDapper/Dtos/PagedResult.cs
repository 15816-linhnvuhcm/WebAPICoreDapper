using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPICoreDapper.Models;

namespace WebAPICoreDapper.Dtos
{
    public class PagedResult
    {
        public List<Product> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRow { get; set; }
    }
}
