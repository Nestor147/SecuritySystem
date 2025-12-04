using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.GetQueryFilter.Autorization
{
    public class GetRoleMenuQueryFilter
    {
        public string ApplicationId { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
