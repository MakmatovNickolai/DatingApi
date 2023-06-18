namespace Common.Models;

public class PaginationRequest
{
    public string? Search { get; set; }
    public string? SortField { get; set; }
    public string? SortDirection { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class PaginationRequest<T> : PaginationRequest
{
    public T? Filter { get; set; }
}
