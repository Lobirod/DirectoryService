using DirectoryService.Application.Positions.CreatePosition;
using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResults;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(201)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody]CreatePositionRequest request,
        [FromServices] CreatePositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);
        return await handler.Handle(command, cancellationToken);
    }
}