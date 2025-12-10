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
    public class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
    {
        public void Configure(EntityTypeBuilder<LoginAudit> builder)
        {
            builder.ToTable("LoginAudit", "Autenticacion");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.UserId)
                   .HasColumnName("UserId");

            builder.Property(e => e.Username)
                   .HasMaxLength(50)
                   .HasColumnName("Username");

            builder.Property(e => e.IPAddress)
                   .HasMaxLength(100)
                   .HasColumnName("IPAddress");

            builder.Property(e => e.UserAgent)
                   .HasMaxLength(500)
                   .HasColumnName("UserAgent");

            builder.Property(e => e.IsSuccessful)
                   .HasColumnName("IsSuccessful");

            builder.Property(e => e.Message)
                   .HasMaxLength(250)
                   .HasColumnName("Message");

            builder.Property(e => e.LoggedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("LoggedAt");

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

            //builder.HasIndex(e => new { e.Username, e.LoggedAt })
            //       .HasDatabaseName("IX_LoginAudit_Username_LoggedAt");

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.LoginAuditEntries)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
