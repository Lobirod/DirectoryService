using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentIdentifier
{
    private const short MIN_LENGTH = 3;
    private const short MAX_LENGTH = 150;
    public string Value { get; }

    private DepartmentIdentifier(string value)
    {
        Value = value;
    }

    public static Result<DepartmentIdentifier> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentIdentifier>("Идентификатор подразделения не должен быть пустым");
        if (value.Length < MIN_LENGTH)
            return Result.Failure<DepartmentIdentifier>($"Идентификатор подразделения не должен быть менее {MIN_LENGTH} символов");
        if (value.Length > MAX_LENGTH)
            return Result.Failure<DepartmentIdentifier>($"Идентификатор подразделения не должен быть более {MAX_LENGTH} символов");
        if(!Regex.IsMatch(value, @"[^a-zA-Z]"))
            return Result.Failure<DepartmentIdentifier>($"Идентификатор подразделения должен быть только латиницей");

        return new DepartmentIdentifier(value);
    }
}