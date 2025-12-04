using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters
{
    public class RoleQueryFilter : AuditFieldsQueryFilter
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
