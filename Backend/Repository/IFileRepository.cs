using Backend.Entity;

namespace Backend.Repository
{
    public interface IFileRepository
    {
        Task AddAsync(UserFile userFile);
        Task<List<UserFile>> GetFilesByUserIdAsync(int userId);
        Task<UserFile?> GetUserFileAsync(int userId, string fileName);
        void DeleteUserFile(UserFile file);
        Task SaveChangesAsync();
    }
}
