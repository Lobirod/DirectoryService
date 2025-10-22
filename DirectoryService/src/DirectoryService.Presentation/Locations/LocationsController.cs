using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResults;

namespace DirectoryService.Presentation.Locations;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Result<Guid, Errors>, CreateLocationCommand> handler,
        [FromBody]CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(locationDto.Name, locationDto.Address, locationDto.Timezone);
        return await handler.Handle(command, cancellationToken);
    }
}