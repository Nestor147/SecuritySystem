using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.GetQueryFilter.Autorization
{
    public class GetRoleQueryFilter : PaginationQueryFilter
    {
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ResultType { get; set; }
    }
}
