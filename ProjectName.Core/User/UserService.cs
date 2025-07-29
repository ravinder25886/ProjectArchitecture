

using ProjectName.DataAccess.User;
using ProjectName.Models.Account;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.Core.User;
public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<BaseResponse<int>> AddAsync(UserModel user)
    {
        return await _userRepository.CreateAsync(user);
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<BaseResponse<IEnumerable<UserModel>>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<BaseResponse<UserModel>> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public Task<BaseResponse<int>> UpdateAsync(UserModel user)
    {
        return _userRepository.UpdateAsync(user);
    }
}
