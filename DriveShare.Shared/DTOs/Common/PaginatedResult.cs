namespace DriveShare.Shared.DTOs.Common
{
    public class PaginatedResult<T>
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 0;
        public List<T> Items { get; set; } = new List<T>();
    }
}
