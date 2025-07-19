using ProjectName.Domain.Account;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.Infrastructure.User;

//Add CRUD methods for dataabse
public interface IUserService
{  // Create
    Task<BaseResponse<int>> CreateUserAsync(UserModel User);
    // Read
    Task<UserModel?> GetUserByIdAsync(int id);
    Task<BaseResponse<IEnumerable<UserModel>>> GetAllUsersAsync();
    // Update
    Task<BaseResponse<int>> UpdateUserAsync(UserModel user);
    // Delete
    Task<BaseResponse<bool>> DeleteUserAsync(int id);
}
