using AccountService.Infrastructure.DB.Contexts;
using AccountService.Infrastructure.DB.Initialize;
using AccountService.Infrastructure.DB.Repositories;
using CustomHelper.Middlewares;
using CustomHelper.PipelineBehavior;
using Duende.IdentityServer.EntityFramework.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AccountService.API2.Configures
{
    public static class Configure
    {
        public static void SeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                var userDbContext = scope.ServiceProvider.GetService<UserDbContext>();

                DataBaseInitialize.EnsureSeedData(configurationDbContext, persistedGrantDbContext, userDbContext);
            }
        }
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDataBases(connectionString);

            builder.Services.AddRedis(builder.Configuration);

            builder.Services.AddMapper();

            builder.Services.AddRedis(builder.Configuration);
            
            builder.Services.AddMeditor();

            builder.Services.ConfugureCookies();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            SeedData(app);
            app.UseAuthorization();

            return app;
        }

        private static IServiceCollection ConfugureCookies(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddCookie("IdentityServer.Cookie", options =>
                {
                    options.Cookie.Name = "IdentityServer.Cookie";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);

                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";
                });

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
            });

            return services;
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
                // this is something you will want in production to reduce load on and requests to the DB
                //.AddConfigurationStoreCache()
                //
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, dbOpts =>
                        dbOpts.MigrationsAssembly(typeof(AccountService.Infrastructure.DB.Contexts.UserDbContext).Assembly.FullName));
                });


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
    }
}
