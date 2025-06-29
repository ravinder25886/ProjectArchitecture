namespace ProjectName.Infrastructure.User;

//Add CRUD methods for dataabse
public interface IUserRepository
{  // Create
    Task<int> CreateUserAsync(UserModel user);

    // Read
    Task<UserModel?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserModel>> GetAllUsersAsync();

    // Update
    Task<bool> UpdateUserAsync(UserModel user);

    // Delete
    Task<bool> DeleteUserAsync(int id);
}
