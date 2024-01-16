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
    public class ProfilePictureConfig : IEntityTypeConfiguration<ProfilePicture>
    {
        public void Configure(EntityTypeBuilder<ProfilePicture> builder)
        {
            builder
                .Property(x => x.Id)
                .HasAnnotation("Key", true);

            builder
                .HasIndex(x => x.Name, "IX_Name");

            builder
                .Property(p => p.CreationDate)
                .HasDefaultValueSql("getdate()");

            builder
                .HasOne(x => x.User)
                .WithMany(u => u.ProfilePicture)
                .HasForeignKey(x => x.UserId);
        }
    }
}
