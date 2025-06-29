
using ProjectName.Infrastructure.Connections;

namespace ProjectName.Infrastructure.User;
public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }
    public Task<int> CreateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserModel>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserModel?> GetUserByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }
}
