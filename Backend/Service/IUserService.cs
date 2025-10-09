namespace Backend.Service
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ChangeEmailAsync(int userId, string newEmail, string password);
        Task<bool> DeleteAccountAsync(int userId, string password);
    }
}
