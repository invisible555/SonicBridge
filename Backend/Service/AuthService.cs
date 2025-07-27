using Backend.DTO;
using Backend.Entity;
using Backend.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Backend.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;
        private readonly int _accessTokenLiveTime;
        private readonly int _refreshTokenLiveTime;
        public AuthService(IUserRepository userRepository, PasswordHasher<User> passwordHasher, JwtService jwtService, IConfiguration config, IUserTokenRepository userTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _accessTokenLiveTime = int.TryParse(config["Jwt:AccessTokenLifetimeMinutes"], out var minutes) ? minutes : 15;
            _refreshTokenLiveTime = int.TryParse(config["Jwt:RefreshTokenLifetimeDays"], out var days) ? days : 15;
            _config = config;
            _userTokenRepository = userTokenRepository;
        }

        public async Task<AuthenticationResultDTO> Authenticate(string? login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return new AuthenticationResultDTO
                {
                    Success = false,
                    ErrorMessage = "Login or email is required."
                };
            }
            User? user=null;
              user = await _userRepository.GetUserByLoginAsync(login);
            if (user ==null)
                user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null)
            {                 return new AuthenticationResultDTO
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if(result == PasswordVerificationResult.Failed)
            {
                return new AuthenticationResultDTO
                {
                    Success = false,
                    ErrorMessage = "Invalid password"
                };
            }
            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Login, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var accessTokenEntity = new UserAccessToken
            {
                UserId = user.Id,
                Token = accessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime),
            };

            var refreshTokenEntity = new UserRefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenLiveTime),
            };

            // save tokens to the database
            await _userTokenRepository.AddAccessTokenAsync(accessTokenEntity);
            await _userTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
            await _userRepository.SaveChangesAsync();
            return new AuthenticationResultDTO
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryTime = accessTokenEntity.ExpiryDate,
                Login = user.Login,
                Role = user.Role,
            };
        }

        public async Task<RegistrationResultDTO> RegisterUser(RegistrationRequestDTO model)
        { // Sprawdź, czy login już istnieje
            if (await _userRepository.GetUserByLoginAsync(model.Login) != null)
            {
                return new RegistrationResultDTO { Success = false, ErrorMessage = "Login jest zajęty." };
            }

            // Sprawdź, czy email już istnieje
            if (await _userRepository.GetUserByEmailAsync(model.Email) != null)
            {
                return new RegistrationResultDTO { Success = false, ErrorMessage = "Email jest już używany." };
            }

            var user = new User
            {
                Login = model.Login,
                Email = model.Email,
                Role = "user"
            };

            // Hashuj hasło!
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                await _userRepository.AddUserAsync(user);
                await _userRepository.SaveChangesAsync();


                return new RegistrationResultDTO
                {
                    Success = true,
                 
                };
            }
            catch
            {
                return new RegistrationResultDTO
                {
                    Success = false,
                    ErrorMessage = "Wystąpił błąd podczas rejestracji."
                };
            }
        }
        public async Task LogoutAsync(int userId, string refreshToken,string accessToken)
        {
            await _userTokenRepository.RevokeRefreshTokenAsync(userId, refreshToken);
            await _userTokenRepository.RevokeAccessTokenAsync(userId, accessToken);
            await _userTokenRepository.SaveChangesAsync();
        }
        public async Task<AuthenticationResultDTO> RefreshAccessTokenAsync(string refreshToken)
        {
            // Szukasz refresh tokena w bazie
            var tokenEntity = await _userTokenRepository.GetRefreshTokenAsync(refreshToken);

            if (tokenEntity == null || !tokenEntity.IsActive)
                return new AuthenticationResultDTO { Success = false, ErrorMessage = "Refresh token nieważny." };

            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);
            if (user == null)
                return new AuthenticationResultDTO { Success = false, ErrorMessage = "Nie znaleziono użytkownika." };

            // Generujesz nowy access token
            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Login, user.Role);
            var expiry = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime);

            var accessTokenEntity = new UserAccessToken
            {
                UserId = user.Id,
                Token = accessToken,
                ExpiryDate = expiry,
            };
            await _userTokenRepository.AddAccessTokenAsync(accessTokenEntity);
            await _userTokenRepository.SaveChangesAsync();

            // Możesz tu też wygenerować nowy refresh token jeśli chcesz (rotacja refresh tokenów)
            // albo zostawić stary jeśli Twoja polityka na to pozwala

            return new AuthenticationResultDTO
            {
                Success = true,
                AccessToken = accessToken,
                ExpiryTime = expiry,
                RefreshToken = null // lub nowy refresh token, jeśli rotujesz
            };
        }
    }
}
