
using ProjectName.Domain.Account;
using ProjectName.Infrastructure.Account.Login;

namespace ProjectName.Application.Account.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
           return await _loginRepository.LoginAsync(loginRequest);
        }
    }
}
