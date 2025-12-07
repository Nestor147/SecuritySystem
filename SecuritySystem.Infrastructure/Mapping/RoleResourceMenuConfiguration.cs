using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class RoleResourceMenuConfiguration : IEntityTypeConfiguration<RoleResourceMenu>
    {
        public void Configure(EntityTypeBuilder<RoleResourceMenu> builder)
        {
            builder.ToTable("RoleResourceMenus", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.RoleId)
                   .IsRequired()
                   .HasColumnName("RoleId");

            builder.Property(e => e.ResourceId)
                   .IsRequired()
                   .HasColumnName("ResourceId");

            builder.Property(e => e.RecordStatus)
                   .IsRequired()
                   .HasDefaultValue(1)
                   .HasColumnName("RecordStatus");

            builder.Property(e => e.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("CreatedAt");

            builder.Property(e => e.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasDefaultValueSql("SYSTEM_USER")
                   .HasColumnName("CreatedBy");

            //builder.HasIndex(e => new { e.RoleId, e.ResourceId })
            //       .IsUnique()
            //       .HasDatabaseName("UQ_RoleResourceMenus_Role_Resource");

            //builder.HasOne(e => e.Role)
            //       .WithMany(r => r.RoleResourceMenus)
            //       .HasForeignKey(e => e.RoleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(e => e.Resource)
            //       .WithMany(r => r.RoleResourceMenus)
            //       .HasForeignKey(e => e.ResourceId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
