using AutoMapper;
using Vroom.Service.Database.Entities;
using Vroom.Service.Pagination;

namespace Vroom.Service.Extensions;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> FilterAsyncEnumerable<T>(
        this IAsyncEnumerable<T> enumerable,
        string? searchQuery
    )
        where T : IVehicle
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return enumerable;

        return enumerable.Where(m =>
            m.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || m.Abbreviation.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
        );
    }

    public static IAsyncEnumerable<T> SortAsyncEnumerable<T>(
        this IAsyncEnumerable<T> enumerable,
        Sort? sortBy,
        bool descending
    )
        where T : IVehicle
    {
        return sortBy switch
        {
            Sort.Name => descending
                ? enumerable.OrderByDescending(m => m.Name)
                : enumerable.OrderBy(m => m.Name),
            Sort.Abbreviation => descending
                ? enumerable.OrderByDescending(m => m.Abbreviation)
                : enumerable.OrderBy(m => m.Abbreviation),
            Sort.Id => descending
                ? enumerable.OrderByDescending(m => m.Id)
                : enumerable.OrderBy(m => m.Id),
            _ => enumerable.OrderBy(m => m.Name),
        };
    }

    /// <typeparam name="T1">Type to map from</typeparam>
    /// <typeparam name="T2">Type to map to</typeparam>
    public static async Task<PagedResult<T2>> ToPagedResultAsync<T1, T2>(
        this IAsyncEnumerable<T1> enumerable,
        int pageNumber,
        int pageSize,
        IMapper mapper
    )
        where T1 : IVehicle
    {
        var totalCount = await enumerable.CountAsync();
        var items = await enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<T2>
        {
            Items = mapper.Map<IEnumerable<T2>>(items),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }
}
