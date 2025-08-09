using ProjectName.Models.Account;
using RS.Utilities.BaseResponseModel;

namespace ProjectName.Core.User;
public interface IUserService
{
    public Task<BaseResponse<int>> AddAsync(UserModel user);
    public Task<BaseResponse<int>> UpdateAsync(UserModel user);
    public Task<BaseResponse<bool>> DeleteAsync(int id);
    public Task<BaseResponse<UserModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<UserModel>>> GetAllAsync();
}
