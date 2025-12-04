using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "SECURITY_SYSTEM");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.ExternalUserId)
                   .HasColumnName("ExternalUserId");

            builder.Property(e => e.Username)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("Username");

            builder.Property(e => e.Email)
                   .HasMaxLength(150)
                   .HasColumnName("Email");

            builder.HasIndex(e => e.Username)
                   .IsUnique();

            builder.HasIndex(e => e.Email)
                   .IsUnique();

            builder.Property(e => e.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(300)
                   .HasColumnName("PasswordHash");

            builder.Property(e => e.LastPasswordChange)
                   .HasColumnType("datetime2")
                   .HasColumnName("LastPasswordChange");

            builder.Property(e => e.IsLocked)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("IsLocked");

            builder.Property(e => e.LockDate)
                   .HasColumnType("datetime2")
                   .HasColumnName("LockDate");

            builder.Property(e => e.IsNewUser)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("IsNewUser");

            builder.Property(e => e.KeepLoggedIn)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("KeepLoggedIn");

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
        }
    }
}
