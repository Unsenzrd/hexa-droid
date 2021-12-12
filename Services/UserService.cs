using hexa_droid.Services.Interface;

namespace hexa_droid.Services;

public class UserService : IUserService
{
    private ApiContext _context;

    public UserService(ApiContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        if (!_context.Users.Any())
        {
            _context.Users.AddRange(
            new User
            {
                Id = 1,
                Name = "Josh",
                Email = "joshh@email.com",
                Attributes = new UserAttributes { Age = 29, IsEnabled = true }
            },
            new User
            {
                Id = 2,
                Name = "Toni",
                Email = "tonii@email.com",
                Attributes = new UserAttributes { Age = 29, IsEnabled = true }
            });

            await _context.SaveChangesAsync();
        }

        return _context.Users;
    }

    public async Task<User> GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public Task<User> CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateUserById(User updatedUser)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserById(int id)
    {
        throw new NotImplementedException();
    }
}
