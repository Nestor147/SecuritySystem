using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class SelectedRole
    {
        public int RoleId { get; set; }
        public bool IsSelected { get; set; }
        public bool IsInspector { get; set; }
    }
}
