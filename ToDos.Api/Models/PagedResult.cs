namespace ToDos.Api.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    public class PagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public int Skip => (Page - 1) * PageSize;
        
        public void Validate()
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100; // Max page size limit
        }
    }
}
