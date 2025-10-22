namespace DirectoryService.Domain.ValueObjects;

public record DepartmentPath
{
    private const char Separator = '/';

    public string Value { get; }

    private DepartmentPath(string value)
    {
        Value = value;
    }

    public static DepartmentPath CreateParent(DepartmentIdentifier identifier)
    {
        return new DepartmentPath(identifier.Value);
    }

    public DepartmentPath CreateChildren(DepartmentIdentifier childIdentifier)
    {
        return new DepartmentPath(Value + Separator + childIdentifier.Value);
    }
}