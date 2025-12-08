namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
{
    public class Pagination
    {
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public Pagination()
        {
        }

        public Pagination(PaginatedList<object> list)
        {
            TotalRecords = list.TotalRecords;
            PageSize = list.PageSize;
            CurrentPage = list.CurrentPage;
            TotalPages = list.TotalPages;
            HasPreviousPage = list.HasPreviousPage;
            HasNextPage = list.HasNextPage;
        }
    }
}