namespace ProjectName.Models.Account;
public class LoginRequest
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
public class LoginResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = default!;
    public int RoleId { get; set; }

}
