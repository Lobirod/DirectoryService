using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationTimezone
{
    public string Value { get; }

    private LocationTimezone(string value)
    {
        Value = value;
    }

    public static Result<LocationTimezone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(null, "Временная зона IANA не должна быть пустой");

        if (value.Length > LengthConstants.LOCATION_TIMEZONE_MAX_LENGTH)
            return Error.Validation(null, $"Временная зона IANA  не корректна");

        string[] timezoneParts = value.Split("/");

        if (timezoneParts.Length != 2)
            return Error.Validation(null, $"Временная зона IANA  не корректна");

        return new LocationTimezone(value);
    }
}