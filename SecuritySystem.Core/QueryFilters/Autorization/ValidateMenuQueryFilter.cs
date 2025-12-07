using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class ValidateMenuQueryFilter
    {
        public List<string> ResourceIds { get; set; }
        public string RegisteredByUser { get; set; }
    }
}
