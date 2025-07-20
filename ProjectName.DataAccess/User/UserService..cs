
using System.Data;
using System.Reflection;

using Dapper;

using ProjectName.DataAccess.Connections;
using ProjectName.DataAccess.Constants;
using ProjectName.Models.Account;
using ProjectName.Utilities.BaseResponseModel;
using ProjectName.Utilities.Constants;
namespace ProjectName.DataAccess.User;
public class UserService(DapperContext context) : BaseRepository(context), IUserService
{
    public async Task<BaseResponse<int>> CreateUserAsync(UserModel user)
    {
        // ID is marked [IgnoreParam], so won't be sent
        //How to use PROC
       // int id = await _connection.QuerySingleAsync<int>("SaveUser", user.ToParametersForInsert(), commandType: CommandType.StoredProcedure);
        //How to Use SQL query
        int id = await InsertAsync(user, DbUser.UserTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("user") };
    }

    public async Task<BaseResponse<bool>> DeleteUserAsync(int id)
    {
       await _connection.ExecuteAsync(
           "sp_DeleteEmployee",
           new { Id = id },
           commandType: CommandType.StoredProcedure
       );
        return new BaseResponse<bool> { IsSuccess = true, Message = MainMessages.SubjectDeletedSuccess("user") };
    }

    public async Task<BaseResponse<IEnumerable<UserModel>>> GetAllUsersAsync()
    {
        DynamicParameters parameters = new DynamicParameters();
        //parameters.Add("@SearchText","search text");// If we want then we can add Parameters also 
        return new BaseResponse<IEnumerable<UserModel>> { IsSuccess = true, Data = await _connection.QueryAsync<UserModel>("GetAllUsers", parameters, commandType: CommandType.Text) };
    }

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        return await _connection.QueryFirstOrDefaultAsync<UserModel>(
          "GetUserById",
          new { Id = id },
          commandType: CommandType.StoredProcedure
      );
    }

    public async Task<BaseResponse<int>> UpdateUserAsync(UserModel user)
    {
        await _connection.ExecuteAsync("SaveUser", user.ToParametersForUpdate(), commandType: CommandType.StoredProcedure);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("user") };
    }
}
