using SecuritySystem.Core.QueryFilters.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.QueryFilters.Autorization.Request
{
    public class ApplicationsByUserRequest : AuditFieldsQueryFilter
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
    }
}
