using SecuritySystem.Core.QueryFilters.Custom;
using System.Collections.Generic;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class UserRoleQueryFilter : AuditFieldsQueryFilter
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        // En BD es IsInspector (bit), aquí lo tienes como string (ej: "SI"/"NO")
        public string Inspector { get; set; }

        public string DocumentNumber { get; set; }
        public string LastNames { get; set; }
        public string FirstNames { get; set; }
        public string RoleName { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }

        public List<UserRoleQueryFilter> Roles { get; set; }
    }
}
