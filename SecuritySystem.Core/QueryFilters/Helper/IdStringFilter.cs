using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Helper
{
    public class IdStringFilter : PaginationQueryFilter
    {
        public string Id { get; set; }
    }
}
