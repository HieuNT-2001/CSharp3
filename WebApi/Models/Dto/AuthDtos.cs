namespace WebApi.Models.Dto
{
    public record RegisterDto(string Email, string Password, string ConfirmPassword);
    public record LoginDto(string Email, string Password);
    public record TokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiration);
    public record RefreshRequest(string RefreshToken);
    public record LogoutRequest(string RefreshToken);

}
