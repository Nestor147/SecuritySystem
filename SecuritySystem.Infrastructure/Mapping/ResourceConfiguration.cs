using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable("Resources", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.ApplicationId)
                   .IsRequired()
                   .HasColumnName("ApplicationId");

            builder.Property(e => e.Page)
                   .HasMaxLength(50)
                   .HasColumnName("Page");

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("Name");

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(350)
                   .HasColumnName("Description");

            builder.Property(e => e.ResourceType)
                   .IsRequired()
                   .HasColumnName("ResourceType");

            builder.Property(e => e.IconName)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("IconName");

            builder.Property(e => e.IsNew)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasColumnName("IsNew");

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

            //builder.HasOne(e => e.Application)
            //       .WithMany(a => a.Resources)
            //       .HasForeignKey(e => e.ApplicationId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
