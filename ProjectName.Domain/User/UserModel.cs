using ProjectName.Domain.Dapper;

namespace ProjectName.Domain.Account;
public class UserModel:DapperModel
{
    [IgnoreOnInsert]  // Exclude Id during insert
    public int Id { get; set; }  // Primary Key
    [SqlParam("Full_Name")]//Just for ex
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // ⚠️ Hash in real apps
    [IgnoreParam]
    public DateTime CreatedOn { get; set; }

}
