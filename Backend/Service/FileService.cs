using Backend.Entity;
using Backend.Repository;

namespace Backend.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<UserFile> UploadAsync(int userId, IFormFile file, string uploadRootPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Nie wybrano pliku.");

            if (!Directory.Exists(uploadRootPath))
                Directory.CreateDirectory(uploadRootPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadRootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userFile = new UserFile
            {
                UserId = userId,
                FileName = fileName,
                UploadDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _fileRepository.AddAsync(userFile);
            await _fileRepository.SaveChangesAsync();
            return userFile;
        }
        public async Task<List<UserFile>> GetFilesByUserIdAsync(int userId)
        {
            
            return await _fileRepository.GetFilesByUserIdAsync(userId);

        }
    }
}
