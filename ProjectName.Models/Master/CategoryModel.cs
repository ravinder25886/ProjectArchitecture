using System.ComponentModel.DataAnnotations;

using RS.Dapper.Utility.Attributes;

namespace ProjectName.Models.Master;
public class CategoryModel: BaseModel
{
    [MaxLength(255)]
    [SqlParam("Name")]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}
public class CategoryResponse:BaseModel
{
    [SqlParam("Name")]
    public required string Name { get; set; }
}
