using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class UserTokenRepository: IUserTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public UserTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAccessTokenAsync(UserAccessToken token)
        {
            await _dbContext.UserAccessTokens.AddAsync(token);
        }
        public async Task AddRefreshTokenAsync(UserRefreshToken token)
        {
            await _dbContext.UserRefreshTokens.AddAsync(token);
        }
        public async Task<UserRefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }
        public async Task<UserAccessToken?> GetAccessTokenAsync(string token)
        {
            return await _dbContext.UserAccessTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }
        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await _dbContext.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken && !x.IsRevoked);

            if (token != null)
            {
                token.IsRevoked = true;
            }
        }

        public async Task RevokeAccessTokenAsync(int userId, string accessToken)
        {
            var token = await _dbContext.UserAccessTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == accessToken && !x.IsRevoked);

            if (token != null)
            {
                token.IsRevoked = true;

            }
        }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

    }

}
