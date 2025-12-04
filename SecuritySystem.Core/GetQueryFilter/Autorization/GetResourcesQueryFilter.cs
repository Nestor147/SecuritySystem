using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.GetQueryFilter.Autorization
{
    public class GetResourcesQueryFilter : PaginationQueryFilter
    {
        public string ModuleId { get; set; }
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public int ResourceType { get; set; }
        public string IconName { get; set; }
        public string IsNew { get; set; }
        public string ResultType { get; set; }
        public string? IsGhost { get; set; }
    }

}
