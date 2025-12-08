using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class SearchUsersQueryFilter
    {
        public string? SearchCriteria { get; set; }
        public bool OnlyActive { get; set; } = true;
        public int Top { get; set; } = 50;
    }
}
