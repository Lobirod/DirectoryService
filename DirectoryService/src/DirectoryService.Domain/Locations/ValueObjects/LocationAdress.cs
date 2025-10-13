using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationAdress
{
    //EF Core
    private LocationAdress()
    {

    }

    public string Country { get; }
    public string City { get; }
    public string Street { get; }

    private LocationAdress(
        string country,
        string city,
        string street)
    {
        Country = country;
        City = city;
        Street = street;
    }

    public static Result<LocationAdress> Create(
        string country,
        string city,
        string street)
    {
        if (country.Length > LengthConstants.LOCATION_ADRESS_MAX_LENGTH)
        {
            return Result.Failure<LocationAdress>(
                $"Название страны не должен быть более {LengthConstants.LOCATION_ADRESS_MAX_LENGTH} символов");
        }

        if (city.Length > LengthConstants.LOCATION_ADRESS_MAX_LENGTH)
        {
            return Result.Failure<LocationAdress>(
                $"Название города не должен быть более {LengthConstants.LOCATION_ADRESS_MAX_LENGTH} символов");
        }

        if (street.Length > LengthConstants.LOCATION_ADRESS_MAX_LENGTH)
        {
            return Result.Failure<LocationAdress>(
                $"Название улицы не должен быть более {LengthConstants.LOCATION_ADRESS_MAX_LENGTH} символов");
        }

        return new LocationAdress(country, city, street);
    }
}