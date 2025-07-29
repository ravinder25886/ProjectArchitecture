using RS.Dapper.Utility.Attributes;

namespace ProjectName.Models;
public class BaseModel
{
    [IgnoreOnInsert]  // Exclude Id during insert
    public virtual long Id { get; set; }
    public bool IsActive { get; set; }
}
