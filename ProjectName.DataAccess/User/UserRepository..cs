
using System.Data;
using Dapper;
using ProjectName.Models.Account;
using RS.Utilities.BaseResponseModel;
using RS.Utilities.Constants;
using RS.Dapper.Utility.Repositories.DapperExecutor;
using ProjectName.DataAccess.Constants;

namespace ProjectName.DataAccess.User;
public class UserRepository(IDapperExecutor dapperExecutor) : IUserRepository
{
    private readonly IDapperExecutor _dapperExecutor = dapperExecutor;
    public async Task<BaseResponse<int>> CreateAsync(UserModel user)
    {
        // ID is marked [IgnoreParam], so won't be sent
        await _dapperExecutor.ExecuteAsync(DbSchema.UserInsertProc, user.ToParametersForInsert());
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("user") };
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        await _dapperExecutor.ExecuteAsync(
           DbSchema.UserDeleteProc,
           new { Id = id },
           commandType: CommandType.StoredProcedure
       );
        return new BaseResponse<bool> { IsSuccess = true, Message = MainMessages.SubjectDeletedSuccess("user") };
    }

    public async Task<BaseResponse<IEnumerable<UserModel>>> GetAllAsync()
    {
        DynamicParameters parameters = new DynamicParameters();
        //parameters.Add("@SearchText","search text");// If we want then we can add Parameters also 
        return new BaseResponse<IEnumerable<UserModel>> { IsSuccess = true, Data = await _dapperExecutor.QueryAsync<UserModel>(DbSchema.UserGetAllProc, parameters) };
    }

    public async Task<BaseResponse<UserModel>> GetByIdAsync(int id)
    {
        var result = await _dapperExecutor.QueryFirstOrDefaultAsync<UserModel>(DbSchema.UserGetByIdProc, new { Id = id });
        return BaseResponse<UserModel>.FromData(result, null);
    }

    public async Task<BaseResponse<int>> UpdateAsync(UserModel user)
    {
        await _dapperExecutor.ExecuteAsync(DbSchema.UserUpdateProc, user.ToParametersForUpdate());
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("user") };
    }
}
