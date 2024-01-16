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
    public class GroupConfig : IEntityTypeConfiguration<GroupEntity>
    {
        public void Configure(EntityTypeBuilder<GroupEntity> builder)
        {
            builder
                .ToTable("Group");

            builder
                .Property(x => x.Id)
                .HasAnnotation("Key", true);

            builder
                .HasIndex(x => x.Name, "IX_Name");

            builder
                .Property(x => x.Description)
                .HasColumnType("nvarchar(max)");

            builder
                .HasOne(x => x.Department)
                .WithMany(d => d.Groups)
                .HasForeignKey(g => g.Id);

            builder
                .HasMany(x => x.Users)
                .WithMany(u => u.Groups);

            builder
                .Property(g => g.CreationDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
