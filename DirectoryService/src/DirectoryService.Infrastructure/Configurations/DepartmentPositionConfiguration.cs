using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration: IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_position");

        builder.HasKey(dp => dp.Id).HasName("pk_department_position");
        builder.Property(dp => dp.Id)
            .HasConversion(
                dp => dp.Value,
                value => DepartmentPositionId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(dp => dp.DepartmentId)
            .HasConversion(
                dp => dp.Value,
                value => DepartmentId.Create(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dp => dp.PositionId)
            .HasConversion(
                dl => dl.Value,
                value => PositionId.Create(value))
            .HasColumnName("position_id")
            .IsRequired();

        builder.HasOne<Department>()
            .WithMany(d => d.DepartmentPositions)
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<Position>()
            .WithMany(p => p.DepartmentPositions)
            .HasForeignKey(dp => dp.PositionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}