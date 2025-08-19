using RS.Dapper.Utility.Attributes;

namespace ProjectName.Models.Account;
public class UserModel: BaseModel
{
    [SqlParam("FullName")]//Just for example, we will use param name in PROC
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // ⚠️ Hash in real apps
    [IgnoreParam]
    public DateTime CreatedOn { get; set; }

}
