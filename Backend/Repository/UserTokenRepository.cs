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
            await _dbContext.UserAccessToken.AddAsync(token);
        }
        public async Task AddRefreshTokenAsync(UserRefreshToken token)
        {
            await _dbContext.UserRefreshToken.AddAsync(token);
        }
        public async Task<UserRefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.UserRefreshToken
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }
        public async Task<UserAccessToken?> GetAccessTokenAsync(string token)
        {
            return await _dbContext.UserAccessToken
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }
        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await _dbContext.UserRefreshToken
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken && !x.IsRevoked);

            if (token != null)
            {
                token.IsRevoked = true;
            }
        }

        public async Task RevokeAccessTokenAsync(int userId, string accessToken)
        {
            var token = await _dbContext.UserAccessToken
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
