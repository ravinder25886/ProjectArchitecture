using ProjectName.Models.Account;

namespace ProjectName.DataAccess.Account.Login;
// Interface defining the contract for login-related database access
public interface ILoginRepository
{
    public Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}
