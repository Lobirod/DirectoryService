namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentPath
{
    private const char SEPARATOR = '.';

    public string Value { get; }

    private DepartmentPath(string value)
    {
        Value = value;
    }
    
    public static DepartmentPath Create(string value)
    {
        return new DepartmentPath(value);
    }

    public static DepartmentPath CreateParent(DepartmentIdentifier identifier)
    {
        return new DepartmentPath(identifier.Value);
    }

    public DepartmentPath CreateChildren(DepartmentIdentifier childIdentifier)
    {
        return new DepartmentPath(Value + SEPARATOR + childIdentifier.Value);
    }
}