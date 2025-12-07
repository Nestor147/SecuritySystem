using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class RoleEndpointConfiguration : IEntityTypeConfiguration<RoleEndpoint>
    {
        public void Configure(EntityTypeBuilder<RoleEndpoint> builder)
        {
            builder.ToTable("RoleEndpoints", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.RoleId)
                   .IsRequired()
                   .HasColumnName("RoleId");

            builder.Property(e => e.ResourceEndpointId)
                   .IsRequired()
                   .HasColumnName("ResourceEndpointId");

            builder.Property(e => e.ServiceType)
                   .HasColumnName("ServiceType");

            builder.Property(e => e.Endpoint)
                   .HasMaxLength(500)
                   .HasColumnName("Endpoint");

            builder.Property(e => e.PageName)
                   .HasMaxLength(350)
                   .HasColumnName("PageName");

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

            builder.HasIndex(e => new { e.RoleId, e.ResourceEndpointId })
                   .IsUnique()
                   .HasDatabaseName("UQ_RoleEndpoints_Role_Endpoint");

            //builder.HasOne(e => e.Role)
            //       .WithMany(r => r.RoleEndpoints)
            //       .HasForeignKey(e => e.RoleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(e => e.ResourceEndpoint)
            //       .WithMany(re => re.RoleEndpoints)
            //       .HasForeignKey(e => e.ResourceEndpointId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
