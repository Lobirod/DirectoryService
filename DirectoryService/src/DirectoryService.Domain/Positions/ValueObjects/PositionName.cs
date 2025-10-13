using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionName
{
    //EF Core
    private PositionName()
    {

    }

    public string Value { get; }

    private PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PositionName>("Имя позиции не должен быть пустым");
        if (value.Length < LengthConstants.POSITION_NAME_MIN_LENGTH)
        {
            return Result.Failure<PositionName>(
                $"Имя позиции не должен быть менее {LengthConstants.POSITION_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.POSITION_NAME_MAX_LENGTH)
        {
            return Result.Failure<PositionName>(
                $"Имя позиции не должен быть более {LengthConstants.POSITION_NAME_MAX_LENGTH} символов");
        }

        return new PositionName(value);
    }
}