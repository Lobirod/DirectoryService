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
                value => value.Value,
                value => new DepartmentLocationId(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(dl => dl.DepartmentId)
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasConversion(
                value => value.Value,
                value => new LocationId(value))
            .HasColumnName("location_id")
            .IsRequired();
    }
}