using ProjectName.Models.Account;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.DataAccess.User;

//Add CRUD methods for dataabse
public interface IUserService
{  // Create
    public Task<BaseResponse<int>> CreateUserAsync(UserModel user);
    // Read
    public Task<UserModel?> GetUserByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<UserModel>>> GetAllUsersAsync();
    // Update
    public Task<BaseResponse<int>> UpdateUserAsync(UserModel user);
    // Delete
    public Task<BaseResponse<bool>> DeleteUserAsync(int id);
}
