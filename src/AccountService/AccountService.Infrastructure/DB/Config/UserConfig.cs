using AccountService.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(x => x.Id)
                .HasAnnotation("Key", true);

            builder
                .HasIndex(x => x.Email, "IX_Email")
                .IsUnique();

            builder
                .HasMany(x => x.Departments)
                .WithMany(g => g.Users);
        }
    }
}
