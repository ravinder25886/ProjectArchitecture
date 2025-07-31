
using System.Data;
using Dapper;
using ProjectName.Models.Account;
using RS.Dapper.Utility.Connections;

namespace ProjectName.DataAccess.Account.Login;
// Implementation using Dapper or any DB access logic
public class LoginRepository(DapperContext context) : ILoginRepository
{
    private readonly DapperContext _context = context;

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        using var connection = _context.CreateConnection();
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserName", loginRequest.UserName, DbType.String);
        parameters.Add("@Password", loginRequest.Password, DbType.String);

        var user = await connection.QueryFirstOrDefaultAsync<LoginResponse>("PROC_Name", parameters, commandType: CommandType.StoredProcedure);

        return user ?? throw new UnauthorizedAccessException("Invalid credentials.");
    }
}
