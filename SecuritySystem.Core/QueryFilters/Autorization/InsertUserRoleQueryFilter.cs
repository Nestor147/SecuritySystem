namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class InsertUserRoleQueryFilter
    {
        public int UserId { get; set; }
        public IEnumerable<SelectedRole> Roles { get; set; }
    }
}
