using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionDescription
{
    public string Value { get; }

    private PositionDescription(string value)
    {
        Value = value;
    }

    public static Result<PositionDescription, Error> Create(string value)
    {
        if (value.Length > LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Описание должности не должен быть более {LengthConstants.POSITION_DESCRIPTION_MAX_LENGTH} символов");
        }

        return new PositionDescription(value);
    }
}