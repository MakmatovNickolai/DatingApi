using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

public static class EnumerableExtensions
{
    public static async Task<PaginationResponse<TEntity>> Paginate<TEntity>(this IQueryable<TEntity> source, PaginationRequest paginationRequest)
    {
        var total = await source.CountAsync();

        var page = (paginationRequest.Page ?? 0) * paginationRequest.PageSize ?? 10;
        var data = await source.Skip(page)
            .Take(paginationRequest.PageSize ?? 10)
            .ToListAsync();

        return new PaginationResponse<TEntity>
        {
            Page = paginationRequest.Page ?? 0,
            PageSize = paginationRequest.PageSize ?? 10,
            Total = total,
            Data = data
        };
    }
}
