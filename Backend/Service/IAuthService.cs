using Backend.DTO;
using Microsoft.AspNetCore.Authentication;

namespace Backend.Service
{
    public interface IAuthService
    {
        Task<AuthenticationResultDTO> Authenticate(string? login, string password);
        Task<RegistrationResultDTO> RegisterUser(RegistrationRequestDTO model);
        Task LogoutAsync(int userId, string refreshToken, string accessToken);
        Task<AuthenticationResultDTO> RefreshAccessTokenAsync(string refreshToken);
    }
}
