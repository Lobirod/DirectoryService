using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration: IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id).HasName("pk_locations");
        builder.Property(l => l.Id)
            .HasConversion(
                l => l.Value,
                value => LocationId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(l => l.Name)
            .HasConversion(
                l => l.Value,
                value => LocationName.Create(value).Value)
            .HasColumnName("name")
            .HasMaxLength(LengthConstants.LOCATION_NAME_MAX_LENGTH)
            .IsRequired();

        builder.OwnsOne(l => l.Address, ab =>
        {
            ab.ToJson("address");
            ab.Property(l => l.Country)
                .HasColumnName("country")
                .HasMaxLength(LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
                .IsRequired();

            ab.Property(l => l.City)
                .HasColumnName("city")
                .HasMaxLength(LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
                .IsRequired();

            ab.Property(l => l.Street)
                .HasColumnName("street")
                .HasMaxLength(LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(l => l.Timezone)
            .HasConversion(
                l => l.Value,
                value => LocationTimezone.Create(value).Value)
            .HasColumnName("timezone")
            .HasMaxLength(LengthConstants.LOCATION_TIMEZONE_MAX_LENGTH)
            .IsRequired();

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);
    }
}