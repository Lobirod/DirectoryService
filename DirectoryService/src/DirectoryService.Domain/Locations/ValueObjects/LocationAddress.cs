using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationAddress
{
    //EF Core
    private LocationAddress()
    {

    }

    public string Country { get; }
    public string City { get; }
    public string Street { get; }

    private LocationAddress(
        string country,
        string city,
        string street)
    {
        Country = country;
        City = city;
        Street = street;
    }

    public static Result<LocationAddress, Error> Create(
        string country,
        string city,
        string street)
    {
        if (country.Length > LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Название страны не должен быть более {LengthConstants.LOCATION_ADDRESS_MAX_LENGTH} символов");
        }

        if (city.Length > LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Название города не должен быть более {LengthConstants.LOCATION_ADDRESS_MAX_LENGTH} символов");
        }

        if (street.Length > LengthConstants.LOCATION_ADDRESS_MAX_LENGTH)
        {
            return Error.Validation(
                null,
                $"Название улицы не должен быть более {LengthConstants.LOCATION_ADDRESS_MAX_LENGTH} символов");
        }

        return new LocationAddress(country, city, street);
    }
}