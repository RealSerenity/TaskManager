using TaskManager.Data.Entities;

namespace TaskManager.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        User GetByUsername(String username);
        User GetById(int userId);
        IEnumerable<User> GetAll();
        Task<Tuple<bool, User>> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int userId);
    }
}