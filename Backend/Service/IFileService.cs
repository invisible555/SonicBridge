using Backend.Entity;

namespace Backend.Service
{
    public interface IFileService
    {
        Task<UserFile> UploadAsync(int userId, IFormFile file, string uploadRootPath);
        Task<List<UserFile>> GetFilesByUserIdAsync(int userId);
        Task DeleteFileAsync(int userId, string fileName);
    }
}
