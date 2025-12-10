using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class ApplicationTokenPolicyConfiguration : IEntityTypeConfiguration<ApplicationTokenPolicy>
    {
        public void Configure(EntityTypeBuilder<ApplicationTokenPolicy> builder)
        {
            builder.ToTable("ApplicationTokenPolicies", "Autenticacion");

            // Primary Key (one policy per application)
            builder.HasKey(e => e.ApplicationId);

            builder.Property(e => e.ApplicationId)
                   .HasColumnName("ApplicationId");

            builder.Property(e => e.AccessTokenMinutes)
                   .IsRequired()
                   .HasColumnName("AccessTokenMinutes");

            builder.Property(e => e.RefreshTokenDays)
                   .IsRequired()
                   .HasColumnName("RefreshTokenDays");

            builder.Property(e => e.RequireMfa)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("RequireMfa");

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

            //// One-to-one with Applications
            //builder.HasOne<Applications>()
            //       .WithOne()
            //       .HasForeignKey<ApplicationTokenPolicy>(e => e.ApplicationId)
            //       .HasConstraintName("FK_ApplicationTokenPolicies_Applications");
        }
    }
}
