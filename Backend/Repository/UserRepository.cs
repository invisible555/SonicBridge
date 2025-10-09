using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddUserAsync(User user)
        {
            await _dbContext.User.AddAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAccountAsync(int userId, string password)
        {
            await _dbContext.User
                .Where(u => u.Id == userId && u.Password == password)
                .ExecuteDeleteAsync();
        }
    }
}
