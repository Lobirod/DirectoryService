using DirectoryService.Domain;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration: IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id).HasName("pk_positions");
        builder.Property(p => p.Id)
            .HasConversion(
                value => value.Value,
                value => new PositionId(value))
            .HasColumnName("id")
            .IsRequired();

        builder.ComplexProperty(p => p.Name, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("name")
                .HasMaxLength(LengthConstants.POSITION_NAME_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Description, db =>
        {
            db.Property(d => d.Value)
                .HasColumnName("description")
                .HasMaxLength(LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH)
                .IsRequired();
        });

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