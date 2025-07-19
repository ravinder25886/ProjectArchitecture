
using System.Data;
using Dapper;
using ProjectName.Domain.Account;
using ProjectName.Infrastructure.Connections;
using ProjectName.Utilities.BaseResponseModel;
using ProjectName.Utilities.Constants;

namespace ProjectName.Infrastructure.User;
public class UserService(DapperContext context) : BaseRepository(context), IUserService
{
    public async Task<BaseResponse<int>> CreateUserAsync(UserModel user)
    {
        // ID is marked [IgnoreParam], so won't be sent
        int id = await Database.QuerySingleAsync<int>("SaveUser", user.ToParametersForInsert(), commandType: CommandType.StoredProcedure);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("user") };
    }

    public async Task<BaseResponse<bool>> DeleteUserAsync(int id)
    {
       await Database.ExecuteAsync(
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
        return new BaseResponse<IEnumerable<UserModel>> { IsSuccess = true, Data = await Database.QueryAsync<UserModel>("GetAllUsers", parameters, commandType: CommandType.Text) };
    }

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        return await Database.QueryFirstOrDefaultAsync<UserModel>(
          "GetUserById",
          new { Id = id },
          commandType: CommandType.StoredProcedure
      );
    }

    public async Task<BaseResponse<int>> UpdateUserAsync(UserModel user)
    {
        await Database.ExecuteAsync("SaveUser", user.ToParametersForUpdate(), commandType: CommandType.StoredProcedure);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("user") };
    }
}
