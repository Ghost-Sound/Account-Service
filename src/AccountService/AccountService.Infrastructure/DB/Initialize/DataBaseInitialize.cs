using AccountService.Infrastructure.DB.Config.Identity;
using AccountService.Infrastructure.DB.Contexts;
using CustomHelper.Authentication.Enums;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.DB.Initialize
{
    public static class DataBaseInitialize
    {
        public static async Task EnsureSeedData(
            ConfigurationDbContext configurationDbContext, 
            PersistedGrantDbContext persistedGrantDbContext, 
            UserDbContext userDbContext,
            RoleManager<IdentityRole<Ulid>> roleManager)
        {
            if(userDbContext.Database.GetPendingMigrations().Any())
            {
                userDbContext.Database.Migrate();
            }

            if (persistedGrantDbContext.Database.GetPendingMigrations().Any())
            {
                persistedGrantDbContext.Database.Migrate();
            }

            if (configurationDbContext.Database.GetPendingMigrations().Any())
            {
                configurationDbContext.Database.Migrate();
            }

            EnsureSeedData(configurationDbContext);

            await EndureSeedDataUser(userDbContext, roleManager);
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in IdentityServerConfig.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach(var resource in IdentityServerConfig.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityServerConfig.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in IdentityServerConfig.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityProviders.Any())
            {
                context.IdentityProviders.Add(new OidcProvider
                {
                    Scheme = "demoidsrv",
                    DisplayName = "IdentityServer",
                    Authority = "https://demo.duendesoftware.com",
                    ClientId = "login",
                }.ToEntity());
                context.SaveChanges();
            }
        }

        private static async Task EndureSeedDataUser(UserDbContext userDbContext, RoleManager<IdentityRole<Ulid>> roleManager)
        {
            if (!userDbContext.UserRoles.Any())
            {
                var roles = new[] {
                    UserRoles.User.ToString(),
                    UserRoles.Teacher.ToString(),
                    UserRoles.Administrator.ToString(),
                    UserRoles.Student.ToString(),
                    UserRoles.Supervisor.ToString(),
                    };

                foreach (var role in roles)
                {
                    var roleId = Ulid.NewUlid();

                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Ulid>(role) { Id = roleId});
                    }
                }
            }
        }
    }
}
