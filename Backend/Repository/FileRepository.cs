using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class FileRepository: IFileRepository
    {
        private readonly AppDbContext _dbContext;
        public FileRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserFile userFile)
        {
            await _dbContext.UserFile.AddAsync(userFile);


        }
        public async Task<List<UserFile>> GetFilesByUserIdAsync(int userId)
        {
            return await _dbContext.UserFile
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .ToListAsync();
        }
        public async Task<UserFile?> GetUserFileAsync(int userId, string fileName)
        {
            return await _dbContext.UserFile
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FileName == fileName);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public void DeleteUserFile(UserFile file)
        {
            _dbContext.UserFile.Remove(file);
        
        }
    }
}
