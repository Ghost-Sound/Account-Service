using AccountService.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}
