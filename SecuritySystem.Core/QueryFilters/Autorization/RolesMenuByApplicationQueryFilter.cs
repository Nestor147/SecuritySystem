namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class RolesMenuByApplicationQueryFilter
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string? Page { get; set; }
        public string Icon { get; set; }
        public bool IsNew { get; set; }
        public bool IsMenuActive { get; set; }
        public bool IsPageActive { get; set; }
        public string ResourceId { get; set; }
        public int ResourceType { get; set; }
        public string Level { get; set; }
        public int Indentation { get; set; }
        public int Status { get; set; }

        public List<string> Roles { get; set; }
        public List<RolesMenuByApplicationQueryFilter> SubLinks { get; set; }

        public RolesMenuByApplicationQueryFilter()
        {
            SubLinks = new List<RolesMenuByApplicationQueryFilter>();
            Roles = new List<string>();
        }
    }
}
