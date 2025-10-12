using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationTimezone
{
    //EF Core
    private LocationTimezone()
    {

    }

    public string Value { get; }

    private LocationTimezone(string value)
    {
        Value = value;
    }

    public static Result<LocationTimezone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationTimezone>("Временная зона IANA не должна быть пустой");
        if (value.Length > LengthConstants.LOCATION_TIMEZONE_MAX_LENGTH)
            return Result.Failure<LocationTimezone>($"Временная зона IANA  не корректна");
        var timezoneParts = value.Split("/");
        if (timezoneParts.Length != 2)
            return Result.Failure<LocationTimezone>($"Временная зона IANA  не корректна");

        return new LocationTimezone(value);
    }
}