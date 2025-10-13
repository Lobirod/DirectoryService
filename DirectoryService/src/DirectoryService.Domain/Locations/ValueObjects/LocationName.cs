using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationName
{
    //EF Core
    private LocationName()
    {

    }

    public string Value { get; }

    private LocationName(string value)
    {
        Value = value;
    }

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationName>("Имя локации не должен быть пустым");
        if (value.Length < LengthConstants.LOCATION_NAME_MIN_LENGTH)
        {
            return Result.Failure<LocationName>(
                $"Имя локации не должен быть менее {LengthConstants.LOCATION_NAME_MIN_LENGTH} символов");
        }

        if (value.Length > LengthConstants.LOCATION_NAME_MAX_LENGTH)
        {
            return Result.Failure<LocationName>(
                $"Имя локации не должен быть более {LengthConstants.LOCATION_NAME_MAX_LENGTH} символов");
        }

        return new LocationName(value);
    }
}