namespace Common.Models;

public class PaginationResponse<TEntity>
{
    public List<TEntity> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}
