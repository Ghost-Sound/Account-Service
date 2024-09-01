using AccountService.API.Grpc.Service;
using AccountService.Application.Interfaces;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AccountService.API.GrpcServices
{
    public class AccountGrpc(
        IAccountAuthenticationService authenticationService,
        IRegistrationService registrationService,
        ITokenService tokenService,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        ISignInKeys signInKeys) : AccountAuthenticationService.AccountAuthenticationServiceBase
    {
        public override async Task<TokenResponse> Login(UserLogin request, ServerCallContext context)
        {
            var result = await authenticationService.Login(new UserLoginDTO
            {
                Email = request.Email,
                Password = request.Password,
                RememberLogin = request.RememberLogin,
            });

            return await Task.FromResult(new TokenResponse()
            {
                AccessToken = result.Item1,
                RefreshToken = result.Item2,
                UserId = result.Item3,
            });
        }

        public override async Task<RegisterResponse> Register(UserRegistry request, ServerCallContext context)
        {
            await registrationService.Register(new UserRegistryDTO
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                PhoneNumber = request.PhoneNumber,
                Username = request.UserName,
            });

            return await Task.FromResult(new RegisterResponse()
            {
                User = request
            });
        }

        public override async Task<TokenResponse> RefreshToken(RefreshTokenModel request, ServerCallContext context)
        {
            var refreshTokenResponse = await tokenService.RefreshTokenAsync(request.Token, context.CancellationToken);

            return await Task.FromResult(new TokenResponse()
            {
                AccessToken = refreshTokenResponse.AccessToken,
                RefreshToken = refreshTokenResponse.RefreshToken,
            });
        }
    }
}
