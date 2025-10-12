namespace DirectoryService.Domain.DepartmentPositions.ValueObjects;

public record DepartmentPositionId
{
    //EF Core
    private DepartmentPositionId()
    {

    }

    private DepartmentPositionId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get;  }

    public static DepartmentPositionId CreateNew() => new(Guid.NewGuid());

    public static DepartmentPositionId CreateEmpty() => new(Guid.Empty);

    public static DepartmentPositionId Create(Guid id) => new(id);
}