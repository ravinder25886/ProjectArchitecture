using ProjectName.Domain.Account;

namespace ProjectName.Application.Account.Login;
public interface ILoginService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
}
