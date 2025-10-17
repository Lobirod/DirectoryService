using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationHandler: ICommandHandler<Guid, CreateLocationCommand>
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

    public async Task<Guid> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationId = LocationId.CreateNew();

        var locationName = LocationName.Create(command.Name).Value;

        var locationAddress = LocationAddress.Create(
            command.Address.Country,
            command.Address.City,
            command.Address.Street).Value;

        var locationTimezone = LocationTimezone.Create(command.Timezone).Value;

        var location = Location.Create(
            locationId,
            locationName,
            locationAddress,
            locationTimezone).Value;

        await _locationsRepository.AddAsync(location, cancellationToken);
        _logger.LogInformation("Location created with id {locationId}", locationId);

        return locationId.Value;
    }
}