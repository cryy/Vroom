namespace Vroom.Service.Pagination;

public class FilteringParams
{
    public string? SearchQuery { get; set; }
    public Sort? SortBy { get; set; } = Sort.Name;
    public bool Descending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public void Deconstruct(
        out string? searchQuery,
        out Sort? sortBy,
        out bool descending,
        out int pageNumber,
        out int pageSize
    )
    {
        searchQuery = SearchQuery;
        sortBy = SortBy;
        descending = Descending;
        pageNumber = PageNumber;
        pageSize = PageSize;
    }
}
