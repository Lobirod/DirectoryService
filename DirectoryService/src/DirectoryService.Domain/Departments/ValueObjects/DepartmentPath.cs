namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentPath
{
    private const char SEPARATOR = '.';
    
    private const string DELETED_MARK = "deleted_";

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
    
    public static DepartmentPath SetAsDeleted(string currentPath, DepartmentId? parentId)
    {
        string path;
       
        if (parentId is null)
        {
            path = DELETED_MARK + currentPath;
        }
        else
        {
            string[] segments = currentPath.Split(SEPARATOR);
            string updatedSegment = DELETED_MARK + segments.Last();
            path = string.Join(
                SEPARATOR,
                string.Join(SEPARATOR, segments.Take(segments.Length - 1)),
                updatedSegment);
        }

        return new DepartmentPath(path);
    }
}