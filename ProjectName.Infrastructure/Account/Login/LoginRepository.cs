
using System.Data;

using Dapper;

using ProjectName.Domain.Account;
using ProjectName.Infrastructure.Connections;

namespace ProjectName.Infrastructure.Account.Login;
// Implementation using Dapper or any DB access logic
public class LoginRepository : ILoginRepository
{
    private readonly DapperContext _context;

    public LoginRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        const string query = @"SELECT Id, Email, FullName FROM Users WHERE Email = @Email AND Password = @Password";

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserName", loginRequest.UserName, DbType.String);
        parameters.Add("@Password", loginRequest.Password, DbType.String);

        var user = await connection.QueryFirstOrDefaultAsync<LoginResponse>("PROC_Name", parameters, commandType: CommandType.StoredProcedure);

        return user ?? throw new UnauthorizedAccessException("Invalid credentials.");
    }
}
