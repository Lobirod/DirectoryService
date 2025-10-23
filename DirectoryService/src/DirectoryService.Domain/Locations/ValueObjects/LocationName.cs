using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationName
{
    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null, "Имя локации не должен быть пустым");

        if (value.Length < LengthConstants.LOCATION_NAME_MIN_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя локации не должен быть менее {LengthConstants.LOCATION_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.LOCATION_NAME_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Имя локации не должен быть более {LengthConstants.LOCATION_NAME_MAX_LENGTH} символов");
        }

        return new LocationName(value);
    }
}