using System.ComponentModel.DataAnnotations;

namespace ProjectName.Models.Master;
public class CategoryModel: BaseModel
{
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}
