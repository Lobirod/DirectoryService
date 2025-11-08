using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
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
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentPositionId(value))
            .IsRequired();

        builder.Property(dp => dp.DepartmentId)
            .HasColumnName("department_id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .IsRequired();

        builder.Property(dp => dp.PositionId)
            .HasColumnName("position_id")
            .HasConversion(
                value => value.Value,
                value => new PositionId(value))
            .IsRequired();
    }
}