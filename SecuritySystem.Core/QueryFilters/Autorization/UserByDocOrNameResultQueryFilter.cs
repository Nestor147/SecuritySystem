namespace SecuritySystem.Core.QueryFilters.Autorization
{
    public class UserByDocOrNameResultQueryFilter
    {
        public string UserId { get; set; }
        public string? DocumentNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string SystemUserId { get; set; }
    }
}
