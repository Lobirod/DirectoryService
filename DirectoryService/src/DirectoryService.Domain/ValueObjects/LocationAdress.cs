using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationAdress
{
    private const short MAX_LENGTH = 100;

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
        if (country.Length > MAX_LENGTH)
            return Result.Failure<LocationAdress>($"Название страны не должен быть более {MAX_LENGTH} символов");
        if (city.Length > MAX_LENGTH)
            return Result.Failure<LocationAdress>($"Название города не должен быть более {MAX_LENGTH} символов");
        if (street.Length > MAX_LENGTH)
            return Result.Failure<LocationAdress>($"Название улицы не должен быть более {MAX_LENGTH} символов");
        return new LocationAdress(country, city, street);
    }
}