using IdentityModel.Client;

namespace AccountService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> RequestRefreshTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string refreshToken, CancellationToken cancellation);
        Task<TokenRevocationResponse> RevokeTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string accessToken, CancellationToken cancellation);
        Task<string?> GetAccessTokenAsync();

        Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}