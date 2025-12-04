using SecuritySystem.Core.QueryFilters.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class MenuV2QueryFilter : AuditFieldsQueryFilter
    {
        public string ApplicationId { get; set; }
        public List<MenuV2ItemQueryFilter> Menu { get; set; } = new();
    }
}
