using AccountService.Application.Interfaces;
using CustomHelper.Exception;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITokenService = AccountService.Application.Interfaces.ITokenService;

namespace AccountService.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public TokenService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _interaction = interaction;
            _clientStore = clientStore;
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string accessToken, CancellationToken cancellation)
        {
            return await _httpClientFactory.CreateClient().RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = tokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Token = accessToken
            }, cancellation);
        }

        public async Task<TokenResponse> RequestRefreshTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string refreshToken, CancellationToken cancellation)
        {
            return await _httpClientFactory.CreateClient().RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                RefreshToken = refreshToken
            }, cancellation);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
           return await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model, CancellationToken cancellationToken)
        {
            var result = await _interaction.GetAuthorizationContextAsync(model.RefreshToken);
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(result.Client.ClientId);
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var httpClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(_configuration["URI:URL"], cancellationToken);

            var accessToken = await GetAccessTokenAsync();

            if (discoveryDocument.IsError)
            {
                throw new CustomException("Error retrieving discovery document");
            }

            //Revoke AccessToken
            await RevokeTokenAsync(discoveryDocument.TokenEndpoint,
                client.ClientId,
                client.ClientSecrets.FirstOrDefault()?.Value,
                accessToken,
                cancellationToken);

            //Creating a new
            var refreshTokenResponse = await RequestRefreshTokenAsync(discoveryDocument.TokenEndpoint,
                client.ClientId,
                client.ClientSecrets.FirstOrDefault()?.Value,
                model.RefreshToken,
                cancellationToken);

            if(refreshTokenResponse == null)
            {
                throw new CustomException(nameof(refreshTokenResponse));
            }

            return refreshTokenResponse;
        }
    }
}
