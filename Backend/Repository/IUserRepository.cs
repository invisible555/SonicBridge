using Backend.Entity;

namespace Backend.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByLoginAsync(string login);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task DeleteAccountAsync(int userId, string password);
    }
}
