using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class KnownDeviceConfiguration : IEntityTypeConfiguration<KnownDevice>
    {
        public void Configure(EntityTypeBuilder<KnownDevice> builder)
        {
            builder.ToTable("KnownDevices", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasColumnName("UserId");

            builder.Property(e => e.FingerprintHash)
                   .IsRequired()
                   .HasMaxLength(300)
                   .HasColumnName("FingerprintHash");

            builder.Property(e => e.DeviceName)
                   .HasMaxLength(100)
                   .HasColumnName("DeviceName");

            builder.Property(e => e.UserAgent)
                   .HasMaxLength(500)
                   .HasColumnName("UserAgent");

            builder.Property(e => e.IPAddress)
                   .HasMaxLength(100)
                   .HasColumnName("IPAddress");

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

            //builder.HasIndex(e => new { e.UserId, e.FingerprintHash })
            //       .IsUnique()
            //       .HasDatabaseName("UQ_KnownDevices_User_Fingerprint");

            //builder.HasOne(e => e.User)
            //       .WithMany(u => u.KnownDevices)
            //       .HasForeignKey(e => e.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
