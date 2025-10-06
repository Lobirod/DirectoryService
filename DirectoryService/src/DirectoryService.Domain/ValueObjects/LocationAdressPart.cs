using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationAdressPart
{
    public string AdressPart { get;}

    private LocationAdressPart(string value)
    {
        AdressPart = value;
    }

    public static Result<LocationAdressPart> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationAdressPart>("Часть адреса локации не должна быть пустой");
        return new LocationAdressPart(value);
    }
}