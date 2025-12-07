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
    public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(EntityTypeBuilder<RevokedToken> builder)
        {
            builder.ToTable("RevokedTokens", "AUTORIZACION");

            builder.HasKey(e => e.Jti);

            builder.Property(e => e.Jti)
                   .HasColumnType("uniqueidentifier")
                   .HasColumnName("Jti");

            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasColumnName("UserId");

            builder.Property(e => e.Reason)
                   .HasMaxLength(250)
                   .HasColumnName("Reason");

            builder.Property(e => e.RevokedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("RevokedAt");

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
            //       .HasDatabaseName("IX_RevokedTokens_UserId");

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.RevokedTokens)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
