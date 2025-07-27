using Backend.Entity;

namespace Backend.Repository
{
    public interface IUserTokenRepository
    {
        Task AddAccessTokenAsync(UserAccessToken token);
        Task AddRefreshTokenAsync(UserRefreshToken token);
        Task<UserRefreshToken?> GetRefreshTokenAsync(string token);
        Task<UserAccessToken?> GetAccessTokenAsync(string token);
        Task RevokeRefreshTokenAsync(int userId, string refreshToken);
        Task RevokeAccessTokenAsync(int userId, string accessToken);
        Task SaveChangesAsync();
    }
}
