using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Service
{
    public class JwtService
    {
        private readonly string _jwtSecret;
        private readonly IConfiguration _config;
        private readonly int tokenLiveTime;

        public JwtService(IConfiguration config)
        {
            _config = config;
            _jwtSecret = config["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is missing in configuration.");
            tokenLiveTime = int.TryParse(config["Jwt:AccessTokenLifetimeMinutes"], out var minutes) ? minutes : 15;

        }

        public string GenerateAccessToken(string id, string username, string role)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(tokenLiveTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

    }
}
