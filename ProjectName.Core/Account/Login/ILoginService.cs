using ProjectName.Models.Account;

namespace ProjectName.Core.Account.Login;
public interface ILoginService
{
    public Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}
