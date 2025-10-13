using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentIdentifier
{
    //EF Core
    private DepartmentIdentifier()
    {

    }

    public string Value { get; }

    private DepartmentIdentifier(string value)
    {
        Value = value;
    }

    public static Result<DepartmentIdentifier> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentIdentifier>("Идентификатор подразделения не должен быть пустым");

        if (value.Length < LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH)
        {
            return Result.Failure<DepartmentIdentifier>(
                $"Идентификатор подразделения не должен быть менее " +
                $"{LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.DEPARTMENT_IDENTIFIER_MAX_LENGTH)
        {
            return Result.Failure<DepartmentIdentifier>(
                $"Идентификатор подразделения не должен быть более " +
                $"{LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH} символов");
        }

        if(!Regex.IsMatch(value, @"[^a-zA-Z]"))
            return Result.Failure<DepartmentIdentifier>($"Идентификатор подразделения должен быть только латиницей");

        return new DepartmentIdentifier(value);
    }
}