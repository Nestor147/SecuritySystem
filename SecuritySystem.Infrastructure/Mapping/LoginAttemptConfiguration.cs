using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
    {
        public void Configure(EntityTypeBuilder<LoginAttempt> builder)
        {
            builder.ToTable("LoginAttempts", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.UserId)
                   .HasColumnName("UserId");

            builder.Property(e => e.Username)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("Username");

            builder.Property(e => e.IPAddress)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("IPAddress");

            builder.Property(e => e.UserAgent)
                   .HasMaxLength(500)
                   .HasColumnName("UserAgent");

            builder.Property(e => e.IsSuccessful)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("IsSuccessful");

            builder.Property(e => e.AttemptedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("AttemptedAt");

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

            //builder.HasIndex(e => new { e.Username, e.AttemptedAt })
            //       .HasDatabaseName("IX_LoginAttempts_Username_AttemptedAt");

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.LoginAttempts)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
