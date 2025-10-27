using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_position");

        builder.HasKey(dp => dp.Id).HasName("pk_department_position");
        builder.Property(dp => dp.Id)
            .HasConversion(
                value => value.Value,
                value => new DepartmentPositionId(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(dp => dp.DepartmentId)
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dp => dp.PositionId)
            .HasConversion(
                value => value.Value,
                value => new PositionId(value))
            .HasColumnName("position_id")
            .IsRequired();
    }
}