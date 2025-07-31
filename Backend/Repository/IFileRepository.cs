using Backend.Entity;

namespace Backend.Repository
{
    public interface IFileRepository
    {
        Task AddAsync(UserFile userFile);
        Task<List<UserFile>> GetFilesByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
