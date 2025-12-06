using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.GetQueryFilter.Autorization
{
    public class GetApplicationsQueryFilter : PaginationQueryFilter
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ResultType { get; set; }
    }
}
