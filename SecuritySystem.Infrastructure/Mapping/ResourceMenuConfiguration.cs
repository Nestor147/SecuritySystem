using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuritySystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Infrastructure.Mapping
{
    public class ResourceMenuConfiguration : IEntityTypeConfiguration<ResourceMenu>
    {
        public void Configure(EntityTypeBuilder<ResourceMenu> builder)
        {
            builder.ToTable("ResourceMenus", "AUTORIZACION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id");

            builder.Property(e => e.ResourceId)
                   .IsRequired()
                   .HasColumnName("ResourceId");

            builder.Property(e => e.Level)
                   .IsRequired()
                   .HasColumnName("Level");

            builder.Property(e => e.IndentLevel)
                   .IsRequired()
                   .HasColumnName("IndentLevel");

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
            //       .WithMany(r => r.Menus)
            //       .HasForeignKey(e => e.ResourceId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
