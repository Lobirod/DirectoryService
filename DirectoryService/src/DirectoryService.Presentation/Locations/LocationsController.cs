using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Locations;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody]CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(locationDto.Name, locationDto.Address, locationDto.Timezone);
        var locationId = await handler.Handle(command, cancellationToken);
        return Ok(locationId);
    }
}