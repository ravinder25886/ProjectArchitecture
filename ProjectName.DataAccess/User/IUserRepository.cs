using ProjectName.Models.Account;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.DataAccess.User;

//Add CRUD methods for dataabse
public interface IUserRepository
{  // Create
    public Task<BaseResponse<int>> CreateAsync(UserModel user);
    // Read
    public Task<BaseResponse<UserModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<UserModel>>> GetAllAsync();
    // Update
    public Task<BaseResponse<int>> UpdateAsync(UserModel user);
    // Delete
    public Task<BaseResponse<bool>> DeleteAsync(int id);
}
