using DirectoryService.Domain;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id).HasName("pk_departments");
        builder.Property(d => d.Id)
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .HasColumnName("id")
            .IsRequired();

        builder.ComplexProperty(d => d.Name, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("name")
                .HasMaxLength(LengthConstants.DEPARTMENT_NAME_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Identifier, nb =>
        {
            nb.Property(n => n.Value)
                .HasColumnName("identifier")
                .HasMaxLength(LengthConstants.DEPARTMENT_IDENTIFIER_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(d => d.ParentId)
            .HasConversion(
                value => value!.Value,
                value => new DepartmentId(value))
            .HasColumnName("parent_id")
            .IsRequired(false);

        builder.ComplexProperty(d => d.Path, pb =>
        {
            pb.Property(p => p.Value)
                .HasColumnName("path")
                .HasMaxLength(LengthConstants.DEPARTMENT_PATH_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(d => d.Depth)
            .HasColumnName("depth")
            .IsRequired();

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(d => d.ChildrenDepartments)
            .WithOne()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(d => d.DepartmentLocations)
            .WithOne()
            .HasForeignKey(d => d.DepartmentId);

        builder.HasMany(d => d.DepartmentPositions)
            .WithOne()
            .HasForeignKey(d => d.DepartmentId);
    }
}