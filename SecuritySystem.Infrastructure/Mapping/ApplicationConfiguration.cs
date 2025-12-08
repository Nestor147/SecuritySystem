using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Applications>
    {
        public void Configure(EntityTypeBuilder<Applications> builder)
        {
            builder.ToTable("Applications", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.Code)
                   .IsRequired()
                   .HasMaxLength(25)
                   .HasColumnName("Code");

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(250)
                   .HasColumnName("Description");

            builder.Property(e => e.Url)
                   .HasMaxLength(250)
                   .HasColumnName("Url");

            builder.Property(e => e.Icon)
                   .HasMaxLength(50)
                   .HasColumnName("Icon");

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
