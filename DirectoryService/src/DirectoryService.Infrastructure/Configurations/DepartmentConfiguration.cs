using DirectoryService.Domain;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration: IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id).HasName("pk_departments");
        builder.Property(d => d.Id)
            .HasConversion(
                d => d.Value,
                value => DepartmentId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        builder.Property(d => d.Name)
            .HasConversion(
                d => d.Value,
                value => DepartmentName.Create(value).Value)
            .HasColumnName("name")
            .HasMaxLength(LengthConstants.DEPARTMENT_NAME_MAX_LENGTH)
            .IsRequired();

        builder.Property(d => d.Identifier)
            .HasConversion(
                d => d.Value,
                value => DepartmentIdentifier.Create(value).Value)
            .HasColumnName("identifier")
            .HasMaxLength(LengthConstants.DEPARTMENT_IDENTIFIER_MAX_LENGTH)
            .IsRequired();

        builder.OwnsOne(d => d.ParentId, dpb =>
        {
            dpb.Property(d => d.Value)
                .HasColumnName("parent_id");
        });
        builder.Navigation(d => d.ParentId).IsRequired(false);

        builder.Property(d => d.Path)
            .HasColumnName("path")
            .HasMaxLength(LengthConstants.DEPARTMENT_PATH_MAX_LENGTH)
            .IsRequired();

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
            .IsRequired(false);
    }
}