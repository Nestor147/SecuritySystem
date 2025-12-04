using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Custom
{
    public class AuditFieldsQueryFilter
    {
        public string CreatedBy { get; set; } = "SECURITY_SYSTEM";
        public int RecordStatus { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
