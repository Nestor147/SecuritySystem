using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // ✅ Esquema correcto según tu BD actual
            builder.ToTable("RefreshTokens", "Autenticacion");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .HasColumnType("uniqueidentifier")
                   .HasColumnName("Id");

            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasColumnName("UserId");

            // ✅ NUEVA COLUMNA: ApplicationId
            builder.Property(e => e.ApplicationId)
                   .IsRequired()
                   .HasColumnName("ApplicationId");

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

            // ✅ Índices alineados con la BD
            builder.HasIndex(e => e.UserId)
                   .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(e => new { e.UserId, e.ApplicationId })
                   .HasDatabaseName("IX_RefreshTokens_UserId_ApplicationId");

            // ✅ Relación con Users (si tienes la navegación)
            // Descomenta SOLO si tu entidad User tiene colección RefreshTokens
            /*
            builder.HasOne(e => e.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            */
        }
    }
}
