public class PagedRequest
{
    /// <summary>
    /// Page number starting from 1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of records per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Column name to order by
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Optional sort direction: ASC or DESC
    /// </summary>
    public string SortDirection { get; set; } = "ASC";

    /// <summary>
    /// List of filters to apply to the query
    /// </summary>
    public List<SqlFilter> Filters { get; set; } = default!;
}
