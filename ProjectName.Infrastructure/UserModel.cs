namespace ProjectName.Infrastructure;
public class UserModel
{
    public int Id { get; set; }  // Primary Key
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // ⚠️ Hash in real apps
}
