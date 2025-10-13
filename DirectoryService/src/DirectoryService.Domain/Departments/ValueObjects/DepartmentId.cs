namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentId
{
    //EF Core
    private DepartmentId()
    {

    }

    private DepartmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get;  }

    public static DepartmentId CreateNew() => new(Guid.NewGuid());

    public static DepartmentId CreateEmpty() => new(Guid.Empty);

    public static DepartmentId Create(Guid id) => new(id);
}