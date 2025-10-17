using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentName
{
    //EF Core
    private DepartmentName()
    {

    }

    public string Value { get; }

    private DepartmentName(string value)
    {
        Value = value;
    }

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentName>("Имя подразделения не должен быть пустым");
        if (value.Length < LengthConstants.DEPARTMENT_NAME_MIN_LENGTH)
        {
            return Result.Failure<DepartmentName>(
                $"Имя подразделения не должен быть менее {LengthConstants.DEPARTMENT_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.DEPARTMENT_NAME_MAX_LENGTH)
        {
            return Result.Failure<DepartmentName>(
                $"Имя подразделения не должен быть более {LengthConstants.DEPARTMENT_NAME_MAX_LENGTH} символов");
        }

        return new DepartmentName(value);
    }
}