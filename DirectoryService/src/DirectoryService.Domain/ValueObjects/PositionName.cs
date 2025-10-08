using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record PositionName
{
    private const short MIN_LENGTH = 3;
    private const short MAX_LENGTH = 100;
    public string Value { get; }

    private PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PositionName>("Имя позиции не должен быть пустым");
        if (value.Length < MIN_LENGTH)
            return Result.Failure<PositionName>($"Имя позиции не должен быть менее {MIN_LENGTH} символов");
        if (value.Length > MAX_LENGTH)
            return Result.Failure<PositionName>($"Имя позиции не должен быть более {MAX_LENGTH} символов");

        return new PositionName(value);
    }
}