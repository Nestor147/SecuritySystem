namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class InsertRoleResourceQueryFilter
    {
        public string MenuId { get; set; }
        public string ResourceId { get; set; }
        public string RoleId { get; set; }

        /// <summary>
        /// 0 / 1 flag indicating if it is selected.
        /// </summary>
        public int IsSelected { get; set; }
    }
}
