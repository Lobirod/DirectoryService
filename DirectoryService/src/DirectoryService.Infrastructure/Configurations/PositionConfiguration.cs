using DirectoryService.Domain;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id).HasName("pk_positions");
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new PositionId(value))
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasConversion(
                value => value.Value,
                value => PositionName.Create(value).Value)
            .HasMaxLength(LengthConstants.POSITION_NAME_MAX_LENGTH)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasConversion(
                value => value.Value,
                value => PositionDescription.Create(value).Value)
            .HasMaxLength(LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(p => p.DepartmentPositions)
            .WithOne()
            .HasForeignKey(p => p.PositionId);
    }
}