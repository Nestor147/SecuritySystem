using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class ResourceEndpointConfiguration : IEntityTypeConfiguration<ResourceEndpoint>
    {
        public void Configure(EntityTypeBuilder<ResourceEndpoint> builder)
        {
            builder.ToTable("ResourceEndpoints", "SECURITY_SYSTEM");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.ResourceId)
                   .IsRequired()
                   .HasColumnName("ResourceId");

            builder.Property(e => e.ServiceType)
                   .IsRequired()
                   .HasColumnName("ServiceType");

            builder.Property(e => e.Endpoint)
                   .IsRequired()
                   .HasMaxLength(350)
                   .HasColumnName("Endpoint");

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(500)
                   .HasColumnName("Description");

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

            //builder.HasOne(e => e.Resource)
            //       .WithMany(r => r.Endpoints)
            //       .HasForeignKey(e => e.ResourceId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
