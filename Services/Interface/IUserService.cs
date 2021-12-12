namespace hexa_droid.Services.Interface;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> GetUserById(int id);
    Task<User> CreateUser(User user);
    Task<User> UpdateUserById(User updatedUser);
    Task<bool> DeleteUserById(int id);
}
