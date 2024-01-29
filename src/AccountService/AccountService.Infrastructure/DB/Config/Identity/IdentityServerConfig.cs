using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Config.Identity
{
    public class IdentityServerConfig
    {
        private static IConfiguration Configuration { get; }
        private readonly static string _signInPath;
        private readonly static string _logOutPath;
        private readonly static string _postLogOutRedirect;
        private readonly static string _url;

        static IdentityServerConfig()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _url = Configuration["URI:URL"];
            _signInPath = Configuration["URI:SigninPath"];
            _logOutPath = Configuration["URI:LogOutPath"];
            _postLogOutRedirect = Configuration["URI:PostLogOutRedirect"];
        }

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = JwtClaimTypes.Role,
                    DisplayName = ConstantProject.ScopeName.UserRoleDisplayName,
                    Description = "Your user role information",
                    UserClaims = {JwtClaimTypes.Role},
                },
                new IdentityResource
                {
                    Name = ConstantProject.ScopeName.GroupName,
                    DisplayName = ConstantProject.ScopeName.UserGroupDisplayName,
                    Description = "Your user group information",
                    UserClaims = { ConstantProject.ScopeName.GroupName }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
         new ApiResource[]
         {
                new ApiResource
                {
                    Name = ConstantProject.ScopeName.UserManagementName,
                    DisplayName = ConstantProject.ScopeName.UserManagement,
                    ApiSecrets = { new Secret(ConstantProject.ScopeName.UserManagement.Sha256())},
                    Scopes = new List<string>(){ConstantProject.ScopeName.UserManagement},
                    UserClaims = {JwtClaimTypes.Role},
                },
         };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope(ConstantProject.ScopeName.UserManagement, ConstantProject.ScopeName.UserManagement, new [] {JwtClaimTypes.Role}),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = { _url +  _signInPath},
                    FrontChannelLogoutUri = _url + _logOutPath,
                    PostLogoutRedirectUris = { _url + _postLogOutRedirect},
                    

                    AllowedScopes = 
                    { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        ConstantProject.ScopeName.UserManagement,
                        "role"
                    }
                },
                new Client
                {
                    ClientId = "swagger",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-12312313121".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = { _url +  _signInPath},
                    FrontChannelLogoutUri = _url + _logOutPath,
                    PostLogoutRedirectUris = { _url + _postLogOutRedirect},


                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        ConstantProject.ScopeName.UserManagement,
                        "role"
                    }
                },
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    ClientSecrets = {new Secret("js".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    //Need to change URLs when Angular app is created
                    RedirectUris = { _url +  _signInPath},
                    FrontChannelLogoutUri = _url + _logOutPath,
                    PostLogoutRedirectUris = { _url + _postLogOutRedirect},

                    //Refresh Token
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    //30 days in seconds
                    SlidingRefreshTokenLifetime = 2592000,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,

                        ConstantProject.ScopeName.UserManagement,
                    }
                }
            };
    }
}
