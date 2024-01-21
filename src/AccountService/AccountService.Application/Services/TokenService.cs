using AccountService.Application.Interfaces;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    public class TokenService : ITokenService
    {
        private HttpClient _httpClient;
        public TokenService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string accessToken, CancellationToken cancellation)
        {
            return await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = tokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Token = accessToken
            }, cancellation);
        }

        public async Task<TokenResponse> RequestRefreshTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string refreshToken, CancellationToken cancellation)
        {
            return await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                RefreshToken = refreshToken
            }, cancellation);
        }
    }
}
