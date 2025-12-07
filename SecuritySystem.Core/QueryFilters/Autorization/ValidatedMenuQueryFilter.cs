namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class ValidatedMenuQueryFilter
    {
        public string ResourceId { get; set; }
        public string Page { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
