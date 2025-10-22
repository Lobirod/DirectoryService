using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationHandler: ICommandHandler<Result<Guid, Errors>, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationNameResult = LocationName.Create(command.Name);
        if (locationNameResult.IsFailure)
            return locationNameResult.Error.ToErrors();
        var locationName = locationNameResult.Value;

        var locationAddressResult = LocationAddress.Create(
            command.Address.Country,
            command.Address.City,
            command.Address.Street);
        if (locationAddressResult.IsFailure)
            return locationAddressResult.Error.ToErrors();
        var locationAddress = locationAddressResult.Value;

        var locationTimezoneResult = LocationTimezone.Create(command.Timezone);
        if (locationTimezoneResult.IsFailure)
            return locationTimezoneResult.Error.ToErrors();
        var locationTimezone = locationTimezoneResult.Value;

        var location = Location.Create(
            locationName,
            locationAddress,
            locationTimezone).Value;

        await _locationsRepository.AddAsync(location, cancellationToken);
        _logger.LogInformation("Location created with id {locationId}", location.Id);

        return location.Id.Value;
    }
}