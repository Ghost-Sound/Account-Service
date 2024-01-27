using AccountService.Application.Interfaces;
using AccountService.Application.Options;
using AccountService.Application.Services;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AccountService.Infrastructure.DB.Initialize;
using AccountService.Infrastructure.DB.Repositories;
using CustomHelper.Authentication.Enums;
using CustomHelper.Authentication.Interfaces;
using CustomHelper.Authentication.NewFolder;
using CustomHelper.Middlewares;
using CustomHelper.PipelineBehavior;
using CustomHelper.Service.Interfaces;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;


namespace AccountService.API.Configures
{
    public static class Configure
    {
        public static async Task SeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                var userDbContext = scope.ServiceProvider.GetService<UserDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Ulid>>>();

                await DataBaseInitialize.EnsureSeedData(configurationDbContext, persistedGrantDbContext, userDbContext, roleManager);
            }
        }
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddConfIdentity();

            builder.Services.AddDataBases(connectionString);
            
            builder.Services.AddHttpClient();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddRedis(builder.Configuration);

            builder.Services.AddMapper();

            builder.Services.AddRedis(builder.Configuration);

            builder.Services.AddOptions(builder.Configuration);

            builder.Services.ConfigureDependecies();

            builder.Services.AddMeditor();

            builder.Services.AddAuth();

            return builder.Build();
        }

        public static async Task<WebApplication> ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseIdentityServer();
            await SeedData(app);
            app.UseAuthorization();

            return app;
        }

        private static IServiceCollection AddDataBases(this IServiceCollection services, string connectionString)
        {
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, dbOpts =>
                        dbOpts.MigrationsAssembly(typeof(AccountService.Infrastructure.DB.Contexts.UserDbContext).Assembly.FullName));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, dbOpts =>
                        dbOpts.MigrationsAssembly(typeof(AccountService.Infrastructure.DB.Contexts.UserDbContext).Assembly.FullName));
                })
                .AddAspNetIdentity<User>();


            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "Development")
            {
                services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlServer(connectionString)
                           .EnableSensitiveDataLogging());
            }
            else
            {
                services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            return services;
        }

        private static IServiceCollection AddMeditor(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AccountService.Application.Commands.Users.CreateUserCommand).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private static IServiceCollection AddRedis(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDistributedMemoryCache();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisConfig:Connection"];
                options.InstanceName = "Cheched";
            });


            return services;
        }

        private static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Application.Mapper.MapperProfile));

            return services;
        }

        private static IServiceCollection ConfigureDependecies(this IServiceCollection services)
        {
            services.AddScoped<ISignInKeys, SignInKeys>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthenticationServiceMine, AuthenticationServiceMine>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<Application.Interfaces.ITokenService, TokenService>();

            return services;
        }

        private static IServiceCollection AddConfIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Ulid>>(option =>
            {
                
            }).AddEntityFrameworkStores<UserDbContext>()
               .AddDefaultTokenProviders();

            

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                
            });

            return services;
        }

        private static IServiceCollection AddAuth(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            bool httpMetaData = false;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "Development")
            {
                httpMetaData = false;
            }
            else
            {
                httpMetaData = true;
            }

            

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = "oidc";
            })
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
           {
               option.ExpireTimeSpan = TimeSpan.FromDays(30);
               option.Events.OnSigningOut = async e =>
               {
                   
               };
           })
           .AddOpenIdConnect("oidc", option =>
           {
               option.Authority = "https://localhost:7072";
               option.CallbackPath = "/signin-oidc";
               option.ClientId = "interactive";
               option.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
               option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               option.ResponseType = OpenIdConnectResponseType.Code;
               option.RequireHttpsMetadata = httpMetaData;
               option.ResponseMode = "query";
               option.Scope.Clear();

               option.Scope.Add("openid");
               option.Scope.Add("profile");
               option.Scope.Add("offline_access");
               option.Scope.Add("role");
               option.Scope.Add(ConstantProject.ScopeName.UserManagement);

               option.GetClaimsFromUserInfoEndpoint = true;
               option.SaveTokens = true;

           });

            return services;
        }

        private static IServiceCollection AddOptions(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<IdentityServerOptions>(configuration.GetSection("URI"));

            return services;
        }
    }
}
