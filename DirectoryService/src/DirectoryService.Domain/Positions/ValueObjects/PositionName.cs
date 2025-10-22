using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionName
{
    public string Value { get; }

    private PositionName(string value)
    {
        Value = value;
    }

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null, "Имя должности не должен быть пустым");

        if (value.Length < LengthConstants.POSITION_NAME_MIN_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя должности не должно быть менее {LengthConstants.POSITION_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.POSITION_NAME_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя должности не должно быть более {LengthConstants.POSITION_NAME_MAX_LENGTH} символов");
        }

        return new PositionName(value);
    }
}