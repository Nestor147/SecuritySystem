using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.ApplicationId)
                   .IsRequired()
                   .HasColumnName("ApplicationId");

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("Name");

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("Description");

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

            //builder.HasIndex(e => new { e.ApplicationId, e.Name })
            //       .IsUnique()
            //       .HasDatabaseName("UQ_Roles_Application_Name");

            //builder.HasOne(e => e.Application)
            //       .WithMany(a => a.Roles)
            //       .HasForeignKey(e => e.ApplicationId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
