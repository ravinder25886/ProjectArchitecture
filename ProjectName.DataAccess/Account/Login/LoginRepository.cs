using ProjectName.Models.Account;

namespace ProjectName.DataAccess.Account.Login;
// Implementation using Dapper or any DB access logic
public class LoginRepository() : ILoginRepository
{
    public Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        throw new NotImplementedException();
    }
}
