using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.GetQueryFilter.Autorization
{
    public class GetUsersByRoleQueryFilter : PaginationQueryFilter
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string DocumentNumber { get; set; }
        public string LastNames { get; set; }
        public string FirstNames { get; set; }
        public string Role { get; set; }
        public string ModuleId { get; set; }
        public string ApplicationId { get; set; }
        public string ResultType { get; set; }
    }
}
