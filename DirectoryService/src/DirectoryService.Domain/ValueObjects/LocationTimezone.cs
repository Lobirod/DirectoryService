using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationTimezone
{
    private const int MAX_LENGTH = 100;
    public string Value { get; }

    private LocationTimezone(string value)
    {
        Value = value;
    }

    public static Result<LocationTimezone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationTimezone>("Временная зона IANA не должна быть пустой");
        if (value.Length > MAX_LENGTH)
            return Result.Failure<LocationTimezone>($"Временная зона IANA  не корректна");
        var timezoneParts = value.Split("/");
        if (timezoneParts.Length != 2)
            return Result.Failure<LocationTimezone>($"Временная зона IANA  не корректна");

        return new LocationTimezone(value);
    }
}