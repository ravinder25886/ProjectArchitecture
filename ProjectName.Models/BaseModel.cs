using RS.Dapper.Utility.Attributes;

namespace ProjectName.Models;
public class BaseModel: DapperModel
{
    [IgnoreOnInsert]  // Exclude Id during insert
    public virtual long Id { get; set; }
    public bool IsActive { get; set; }
}
public class SearchRequest
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
    public string SeachText { get; set; } = "";
}
