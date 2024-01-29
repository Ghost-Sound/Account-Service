using AccountService.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.DB.Config
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder
                .Property(x => x.Id)
                .HasAnnotation("Key", true);

            builder
                .HasIndex(x => x.Name, "IX_Name")
                .IsUnique();

            builder
                .Property(x => x.Description)
                .HasColumnType("nvarchar(max)");               

            builder
                .Property(d => d.CreationDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
