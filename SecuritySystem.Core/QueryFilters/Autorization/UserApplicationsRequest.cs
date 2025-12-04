using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class UserApplicationsRequest : AuditFieldsQueryFilter
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
