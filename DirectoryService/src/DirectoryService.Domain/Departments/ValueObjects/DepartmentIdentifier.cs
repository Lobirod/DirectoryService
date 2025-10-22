using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentIdentifier
{
    private static readonly Regex _identifierRegex = new("^[a-zA-Z]+$", RegexOptions.Compiled);
    public string Value { get; }

    private DepartmentIdentifier(string value)
    {
        Value = value;
    }

    public static Result<DepartmentIdentifier, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null, "Идентификатор подразделения не должен быть пустым");

        if (value.Length < LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH)
        {
            return Error.Validation(
                null,
                $"Идентификатор подразделения не должен быть менее " +
                $"{LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.DEPARTMENT_IDENTIFIER_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Идентификатор подразделения не должен быть более " +
                $"{LengthConstants.DEPARTMENT_IDENTIFIER_MIN_LENGTH} символов");
        }

        if(_identifierRegex.IsMatch(value) == false)
            return Error.Validation(null, $"Идентификатор подразделения должен быть только латиницей");

        return new DepartmentIdentifier(value);
    }
}