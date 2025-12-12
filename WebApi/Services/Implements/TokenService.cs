using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Data;
using WebApi.Models.Dto;
using WebApi.Models.Entities;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _db;

        public TokenService(IOptions<JwtSettings> settings, UserManager<User> userManager, AppDbContext db)
        {
            _settings = settings.Value;
            _userManager = userManager;
            _db = db;
        }


        public async Task<TokenResponse> GenerateTokensAsync(User user)
        {
            var access = await CreateAccessTokenAsync(user);
            var refresh = await CreateRefreshTokenAsync(user.Id);
            return new TokenResponse(access.TokenString, refresh.Token, access.Expires);
        }


        private async Task<(string TokenString, DateTime Expires)> CreateAccessTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim("email", user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);
        }


        private async Task<RefreshToken> CreateRefreshTokenAsync(string userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refresh = new RefreshToken
            {
                Token = token,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays),
                IsRevoked = false
            };


            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();
            return refresh;
        }


        public async Task<TokenResponse?> RefreshAsync(string refreshToken)
        {
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked);
            if (stored == null) return null;
            if (stored.Expires < DateTime.UtcNow) return null;

            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null) return null;

            stored.IsRevoked = true;
            _db.RefreshTokens.Update(stored);

            var access = await CreateAccessTokenAsync(user);
            var newRefresh = await CreateRefreshTokenAsync(user.Id);

            await _db.SaveChangesAsync();

            return new TokenResponse(access.TokenString, newRefresh.Token, access.Expires);
        }
    }
}
