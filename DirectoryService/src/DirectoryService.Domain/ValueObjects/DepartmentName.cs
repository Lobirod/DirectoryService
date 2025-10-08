using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentName
{
    private const short MIN_LENGTH = 3;
    private const short MAX_LENGTH = 150;
    public string Value { get; }

    private DepartmentName(string value)
    {
        Value = value;
    }

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentName>("Имя подразделения не должен быть пустым");
        if (value.Length < MIN_LENGTH)
            return Result.Failure<DepartmentName>($"Имя подразделения не должен быть менее {MIN_LENGTH} символов");
        if (value.Length > MAX_LENGTH)
            return Result.Failure<DepartmentName>($"Имя подразделения не должен быть более {MAX_LENGTH} символов");

        return new DepartmentName(value);
    }
}