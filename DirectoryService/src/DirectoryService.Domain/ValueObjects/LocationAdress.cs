using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationAdress
{
    private readonly List<LocationAdressPart> _locationAdressParts = [];
    public IReadOnlyList<LocationAdressPart> LocationAdressParts => _locationAdressParts;
    private LocationAdress(List<LocationAdressPart> locationAdressParts)
    {
        _locationAdressParts = locationAdressParts;
    }

    public static Result<LocationAdress> Create(IEnumerable<LocationAdressPart> locationAdressParts)
    {
        List<LocationAdressPart> locationAdress = locationAdressParts.ToList();
        if (locationAdress.Count == 0)
            return Result.Failure<LocationAdress>($"Список частей адреса должен содержать хотябы один элемент");
        return new LocationAdress(locationAdress);
    }
}