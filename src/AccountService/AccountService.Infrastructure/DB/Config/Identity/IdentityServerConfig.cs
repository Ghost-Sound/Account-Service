using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.Extensions.Configuration;

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
                    DisplayName = ConstantProject.ScopeName.UserManagementConst.UserRoleDisplayName,
                    Description = "Your user role information",
                    UserClaims = {JwtClaimTypes.Role},
                },
                new IdentityResource
                {
                    Name = ConstantProject.ScopeName.GroupManagmentConst.UserManagementName,
                    DisplayName = ConstantProject.ScopeName.GroupManagmentConst.GroupManagment,
                    Description = "Your user group information",
                    UserClaims = { ConstantProject.ScopeName.GroupManagmentConst.UserManagementName }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
         new ApiResource[]
         {
                new ApiResource
                {
                    Name = ConstantProject.ScopeName.UserManagementConst.UserManagementName,
                    DisplayName = ConstantProject.ScopeName.UserManagementConst.UserManagement,
                    ApiSecrets = { new Secret(ConstantProject.ScopeName.UserManagementConst.UserManagement.Sha256())},
                    Scopes = new List<string>(){ConstantProject.ScopeName.UserManagementConst.UserManagement},
                    UserClaims = {JwtClaimTypes.Role},
                },
                new ApiResource
                {
                    Name = ConstantProject.ScopeName.GroupManagmentConst.UserManagementName,
                    DisplayName = ConstantProject.ScopeName.GroupManagmentConst.GroupManagment,
                    ApiSecrets = { new Secret(ConstantProject.ScopeName.GroupManagmentConst.GroupManagment.Sha256())},
                    Scopes = new List<string>(){ConstantProject.ScopeName.GroupManagmentConst.GroupManagment},
                    UserClaims = {JwtClaimTypes.Role},
                },
         };

        public static IEnumerable<ApiScope> ApiScopes
        {
            get
            {
                return new List<ApiScope>
                {
                    new ApiScope(ConstantProject.ScopeName.UserManagementConst.UserManagement, ConstantProject.ScopeName.UserManagementConst.UserManagement, new [] {JwtClaimTypes.Role}),
                    new ApiScope(ConstantProject.ScopeName.GroupManagmentConst.GroupManagment, ConstantProject.ScopeName.GroupManagmentConst.GroupManagment, new [] {JwtClaimTypes.Role}),
                };
            }
        }

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
                        ConstantProject.ScopeName.UserManagementConst.UserManagement,
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
                        ConstantProject.ScopeName.UserManagementConst.UserManagement,
                        ConstantProject.ScopeName.GroupManagmentConst.GroupManagment,
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

                        ConstantProject.ScopeName.UserManagementConst.UserManagement,
                    }
                }
            };
    }
}
