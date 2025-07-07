namespace BuildingBlocks.Models
{
    public class PaginationResponseModel<TResponse>(int page, int pageSize, int pageCount, int totalEntities, List<TResponse>? data)
    {
        public List<TResponse>? Data => data;
        public int Page => page;
        public int PageSize => pageSize;
        public int PageCount => pageCount;
        public int TotalEntities => totalEntities;
        public bool HasNext => Page < pageCount;
        public bool HasPrevious => page > 1;
        public bool IsValidPage => Page > 0 && Page <= PageCount;
    }
}
