using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentName
{
    public string Value { get; }

    private DepartmentName(string value)
    {
        Value = value;
    }

    public static Result<DepartmentName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null, "Имя подразделения не должен быть пустым");

        if (value.Length < LengthConstants.DEPARTMENT_NAME_MIN_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя подразделения не должен быть менее {LengthConstants.DEPARTMENT_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.DEPARTMENT_NAME_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя подразделения не должен быть более {LengthConstants.DEPARTMENT_NAME_MAX_LENGTH} символов");
        }

        return new DepartmentName(value);
    }
}