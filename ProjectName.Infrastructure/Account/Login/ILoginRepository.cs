using ProjectName.Domain.Account;

namespace ProjectName.Infrastructure.Account.Login;
// Interface defining the contract for login-related database access
public interface ILoginRepository
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}
