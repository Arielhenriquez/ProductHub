using Microsoft.EntityFrameworkCore;
using ProductHub.Application.Common.Pagination;

namespace ProductHub.Application.Common.Extensions;

public static class PaginationExtension
{
    public static async Task<Paged<T>> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber, CancellationToken cancellationToken) where T : class
    {
        int page = pageNumber <= 0 ? 1 : pageNumber;
        int skip = (page - 1) * pageSize;

        int totalRecords = await query.CountAsync(cancellationToken);
        List<T> items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        return Paged<T>.Create(items, totalRecords, page, pageSize);
    }
    public static Paged<T> Paginate<T>(this ICollection<T> query, int pageNumber, int pageSize) where T : class
    {
        int page = pageNumber <= 0 ? 1 : pageNumber;
        int skip = (page - 1) * pageSize;
        return Paged<T>.Create(totalRecords: query.Count, items: query.Skip(skip).Take(pageSize).ToList(), currentPage: page, pageSize: pageSize);
    }
    public static Paged<T> PaginateInMemory<T>(
        this IEnumerable<T> collection,
        int pageSize,
        int pageNumber) where T : class
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        int totalRecords = collection.Count();
        var pagedItems = collection.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

        return Paged<T>.Create(pagedItems, totalRecords, pageNumber, pageSize);
    }
}
