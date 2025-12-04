using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Custom
{
    public class PaginationQueryFilter
    {
        public int TamanoDePagina { get; set; } = 10;
        public int NumeroDePagina { get; set; } = 1;
    }
}
