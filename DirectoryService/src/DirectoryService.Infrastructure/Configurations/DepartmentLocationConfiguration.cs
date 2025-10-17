using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration: IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_location");

        builder.HasKey(dl => dl.Id).HasName("pk_department_location");
        builder.Property(dl => dl.Id)
            .HasConversion(
                dl => dl.Value,
                value => DepartmentLocationId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(dl => dl.DepartmentId)
            .HasConversion(
                dl => dl.Value,
                value => DepartmentId.Create(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasConversion(
                dl => dl.Value,
                value => LocationId.Create(value))
            .HasColumnName("location_id")
            .IsRequired();

        builder.HasOne<Department>()
            .WithMany(d => d.DepartmentLocations)
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<Location>()
            .WithMany(l => l.DepartmentLocations)
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}