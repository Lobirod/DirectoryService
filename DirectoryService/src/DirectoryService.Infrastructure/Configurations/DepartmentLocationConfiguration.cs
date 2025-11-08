using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_location");

        builder.HasKey(dl => dl.Id).HasName("pk_department_location");
        builder.Property(dl => dl.Id)
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentLocationId(value))
            .IsRequired();

        builder.Property(dl => dl.DepartmentId)
            .HasColumnName("department_id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasColumnName("location_id")
            .HasConversion(
                value => value.Value,
                value => new LocationId(value))
            .IsRequired();
    }
}