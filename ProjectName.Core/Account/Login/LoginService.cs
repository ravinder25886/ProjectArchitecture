
using ProjectName.Models.Account;
using ProjectName.DataAccess.Account.Login;

namespace ProjectName.Core.Account.Login;

public class LoginService(ILoginRepository loginRepository) : ILoginService
{
    private readonly ILoginRepository _loginRepository = loginRepository;

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
       return await _loginRepository.LoginAsync(loginRequest);
    }
}
