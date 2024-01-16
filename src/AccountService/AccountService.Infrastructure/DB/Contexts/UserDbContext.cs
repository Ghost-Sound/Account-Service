using AccountService.Domain.Entity;
using CustomHelper.EFcore.Converter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Contexts
{
    public class UserDbContext : IdentityDbContext<User, IdentityRole<Ulid>, Ulid>
    {
        // Parameterless constructor for design-time
        public UserDbContext() { }

        public UserDbContext(DbContextOptions<UserDbContext> option) : base(option) { }

        #region Db sets
        public DbSet<User> Users { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }
        public DbSet<Department> Departments { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //That's configuration for all properties of type Ulid
            configurationBuilder
                .Properties<Ulid>()
                .HaveConversion<UlidConverter.UlidToStringConverter>()
                .HaveConversion<UlidConverter.UlidToBytesConverter>();
        }
    }
}
