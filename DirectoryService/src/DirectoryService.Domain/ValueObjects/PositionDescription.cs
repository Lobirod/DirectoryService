using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record PositionDescription
{
    private const short MAX_LENGTH = 1000;
    public string Value { get; }

    private PositionDescription(string value)
    {
        Value = value;
    }

    public static Result<PositionDescription> Create(string value)
    {
        if (value.Length > MAX_LENGTH)
            return Result.Failure<PositionDescription>($"Описание позиции не должен быть более {MAX_LENGTH} символов");

        return new PositionDescription(value);
    }
}