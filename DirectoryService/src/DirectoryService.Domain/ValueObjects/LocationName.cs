using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationName
{
    private const short MIN_LENGTH = 3;
    private const short MAX_LENGTH = 120;
    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationName>("Имя локации не должен быть пустым");
        if (value.Length < MIN_LENGTH)
            return Result.Failure<LocationName>($"Имя локации не должен быть менее {MIN_LENGTH} символов");
        if (value.Length > MAX_LENGTH)
            return Result.Failure<LocationName>($"Имя локации не должен быть более {MAX_LENGTH} символов");

        return new LocationName(value);
    }
}