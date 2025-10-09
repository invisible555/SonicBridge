using Backend.Entity;
using Backend.Repository;
using Microsoft.AspNetCore.Identity;

namespace Backend.Service
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordhassher;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordhassher = new PasswordHasher<User>();
        }
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user =  await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            if (_passwordhassher.VerifyHashedPassword(user, user.Password, currentPassword) == PasswordVerificationResult.Failed)
            {
                return false;
            }
            user.Password = _passwordhassher.HashPassword(user, newPassword);
            await _userRepository.SaveChangesAsync();
            return true;


        }
        public async Task<bool> ChangeEmailAsync(int userId, string newEmail, string password)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            if (_passwordhassher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
            {
                return false;
            }
            user.Email = newEmail;
            await _userRepository.SaveChangesAsync();
            return true;

        }
        public async Task<bool> DeleteAccountAsync(int userId, string password)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            if (_passwordhassher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
            {
                return false;
            }
            await _userRepository.DeleteAccountAsync(userId, user.Password);
            await _userRepository.SaveChangesAsync();
            return true;

        }

    }
}
