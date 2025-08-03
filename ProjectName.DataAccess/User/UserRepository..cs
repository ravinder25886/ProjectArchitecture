
using System.Data;

using Dapper;

using ProjectName.Models.Account;
using ProjectName.Utilities.BaseResponseModel;
using ProjectName.Utilities.Constants;
using RS.Dapper.Utility.Connections;
namespace ProjectName.DataAccess.User;
public class UserRepository(DapperContext dapperContext) : IUserRepository
{
    private readonly DapperContext _dapperContext = dapperContext;
    public async Task<BaseResponse<int>> CreateAsync(UserModel user)
    {
        // ID is marked [IgnoreParam], so won't be sent
        using var connection = _dapperContext.CreateConnection();
        await connection.ExecuteAsync(DbSchema.UserInsertProc, user.ToParametersForInsert(), commandType: CommandType.StoredProcedure);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("user") };
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        await connection.ExecuteAsync(
           "User_Delete",
           new { Id = id },
           commandType: CommandType.StoredProcedure
       );
        return new BaseResponse<bool> { IsSuccess = true, Message = MainMessages.SubjectDeletedSuccess("user") };
    }

    public async Task<BaseResponse<IEnumerable<UserModel>>> GetAllAsync()
    {
        using var connection = _dapperContext.CreateConnection();
        DynamicParameters parameters = new DynamicParameters();
        //parameters.Add("@SearchText","search text");// If we want then we can add Parameters also 
        return new BaseResponse<IEnumerable<UserModel>> { IsSuccess = true, Data = await connection.QueryAsync<UserModel>("User_GetAll", parameters, commandType: CommandType.Text) };
    }

    public async Task<BaseResponse<UserModel>> GetByIdAsync(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<UserModel>(DbSchema.UserGetByIdProc, new { Id = id }, commandType: CommandType.StoredProcedure);
        return BaseResponse<UserModel>.FromData(result, null);
    }

    public async Task<BaseResponse<int>> UpdateAsync(UserModel user)
    {
        using var connection = _dapperContext.CreateConnection();
        await connection.ExecuteAsync(DbSchema.UserUpdateProc, user.ToParametersForUpdate(), commandType: CommandType.StoredProcedure);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("user") };
    }
}
