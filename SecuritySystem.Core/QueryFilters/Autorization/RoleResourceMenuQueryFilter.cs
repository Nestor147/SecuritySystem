namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class RoleResourceMenuQueryFilter
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string ResourceId { get; set; }
        public string RoleId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Page { get; set; }

        public string Level { get; set; }
        public int Indentation { get; set; }

        public List<RoleResourceMenuQueryFilter> SubLinks { get; set; }

        public string RecordStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }

        public RoleResourceMenuQueryFilter()
        {
            SubLinks = new List<RoleResourceMenuQueryFilter>();
        }
    }
}
