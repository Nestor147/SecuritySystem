using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class CryptoKeyConfiguration : IEntityTypeConfiguration<CryptoKey>
    {
        public void Configure(EntityTypeBuilder<CryptoKey> builder)
        {
            builder.ToTable("CryptoKeys", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("Name");

            builder.Property(e => e.KeyType)
                   .IsRequired()
                   .HasColumnName("KeyType");

            builder.Property(e => e.Version)
                   .IsRequired()
                   .HasDefaultValue(1)
                   .HasColumnName("Version");

            builder.Property(e => e.ApplicationId)
                   .HasColumnName("ApplicationId");

            builder.Property(e => e.PublicKeyPem)
                   .IsRequired()
                   .HasColumnType("nvarchar(max)")
                   .HasColumnName("PublicKeyPem");

            builder.Property(e => e.EncryptedPrivateKey)
                   .IsRequired()
                   .HasColumnType("varbinary(max)")
                   .HasColumnName("EncryptedPrivateKey");

            builder.Property(e => e.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true)
                   .HasColumnName("IsActive");

            builder.Property(e => e.StartDate)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .HasColumnName("StartDate");

            builder.Property(e => e.EndDate)
                   .HasColumnType("datetime2")
                   .HasColumnName("EndDate");

            builder.Property(e => e.Thumbprint)
                   .HasMaxLength(128)
                   .HasColumnName("Thumbprint");

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

            //builder.HasIndex(e => new { e.Name, e.Version })
            //       .IsUnique()
            //       .HasDatabaseName("UQ_CryptoKeys_Name_Version");

            //builder.HasOne(e => e.Application)
            //       .WithMany(a => a.CryptoKeys)
            //       .HasForeignKey(e => e.ApplicationId)
            //       .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
