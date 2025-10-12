using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionDescription
{
    //EF Core
    private PositionDescription()
    {

    }

    public string Value { get; }

    private PositionDescription(string value)
    {
        Value = value;
    }

    public static Result<PositionDescription> Create(string value)
    {
        if (value.Length > LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH)
        {
            return Result.Failure<PositionDescription>(
                $"Описание позиции не должен быть более {LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH} символов");
        }

        return new PositionDescription(value);
    }
}