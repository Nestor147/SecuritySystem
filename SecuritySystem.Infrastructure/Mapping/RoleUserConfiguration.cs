using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class RoleUserConfiguration : IEntityTypeConfiguration<RoleUser>
    {
        public void Configure(EntityTypeBuilder<RoleUser> builder)
        {
            builder.ToTable("RoleUsers", "SECURITY_SYSTEM");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.RoleId)
                   .IsRequired()
                   .HasColumnName("RoleId");

            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasColumnName("UserId");

            builder.Property(e => e.IsInspector)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("IsInspector");

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

            //builder.HasIndex(e => new { e.RoleId, e.UserId })
            //       .IsUnique()
            //       .HasDatabaseName("UQ_RoleUsers_Role_User");

            //builder.HasOne(e => e.Role)
            //       .WithMany(r => r.RoleUsers)
            //       .HasForeignKey(e => e.RoleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.RoleUsers)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
