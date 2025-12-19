using WebApi.Models.Dto;
using WebApi.Models.Entities;

namespace WebApi.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokensAsync(User user);
        Task<TokenResponse?> RefreshAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}
