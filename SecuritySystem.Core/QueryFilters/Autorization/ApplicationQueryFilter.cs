using SecuritySystem.Core.QueryFilters.Custom;

namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class ApplicationQueryFilter : AuditFieldsQueryFilter
    {
        public string Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
