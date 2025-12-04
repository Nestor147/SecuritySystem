using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.Entities
{
    public class Resource : AuditFields
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }

        public string Page { get; set; }          // route or page name
        public string Name { get; set; }          // short display name
        public string Description { get; set; }   // detailed description

        public int ResourceType { get; set; }     // 1: Node, 2: Page, etc.
        public string IconName { get; set; }
        public bool IsNew { get; set; }
    }
}
