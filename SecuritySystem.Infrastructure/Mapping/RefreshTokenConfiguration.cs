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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", "SECURITY_SYSTEM");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .HasColumnType("uniqueidentifier")
                   .HasColumnName("Id");

            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasColumnName("UserId");

            builder.Property(e => e.TokenHash)
                   .IsRequired()
                   .HasMaxLength(500)
                   .HasColumnName("TokenHash");

            builder.Property(e => e.ExpiresAt)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasColumnName("ExpiresAt");

            builder.Property(e => e.Used)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("Used");

            builder.Property(e => e.Revoked)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("Revoked");

            builder.Property(e => e.TokenCreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("TokenCreatedAt");

            builder.Property(e => e.IPAddress)
                   .HasMaxLength(100)
                   .HasColumnName("IPAddress");

            builder.Property(e => e.UserAgent)
                   .HasMaxLength(500)
                   .HasColumnName("UserAgent");

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

            //builder.HasIndex(e => e.UserId)
            //       .HasDatabaseName("IX_RefreshTokens_UserId");

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.RefreshTokens)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
