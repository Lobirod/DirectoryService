using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
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
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value))
            .IsRequired();

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasConversion(
                value => value.Value,
                value => DepartmentName.Create(value).Value)
            .HasMaxLength(LengthConstants.DEPARTMENT_NAME_MAX_LENGTH)
            .IsRequired();
        
        builder.Property(d => d.Identifier)
            .HasColumnName("identifier")
            .HasConversion(
                value => value.Value,
                value => DepartmentIdentifier.Create(value).Value)
            .HasMaxLength(LengthConstants.DEPARTMENT_IDENTIFIER_MAX_LENGTH)
            .IsRequired();

        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .HasConversion(
                value => value!.Value,
                value => new DepartmentId(value))
            .IsRequired(false);
        
        builder.Property(d => d.Path)
            .HasColumnName("path")
            .HasColumnType("ltree")
            .HasConversion(
                value => value.Value,
                value => DepartmentPath.Create(value))
            .HasMaxLength(LengthConstants.DEPARTMENT_PATH_MAX_LENGTH)
            .IsRequired();
        
        builder.HasIndex(d => d.Path).HasMethod("gist").HasDatabaseName("idx_departments_path");

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