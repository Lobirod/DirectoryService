using DirectoryService.Application.Departments.Commands.Create;
using DirectoryService.Application.Departments.Commands.Move;
using DirectoryService.Application.Departments.Commands.UpdateLocations;
using DirectoryService.Application.Departments.Queries.GetChildrenByParent;
using DirectoryService.Application.Departments.Queries.GetWithChildren;
using DirectoryService.Application.Departments.Queries.GetWithTopPositions;
using DirectoryService.Contracts.Departments.Request;
using DirectoryService.Contracts.Departments.Response;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResults;

namespace DirectoryService.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(201)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] CreateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:Guid}/locations")]
    [ProducesResponseType<Envelope<Guid>>(201)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<Guid>> Update(
        [FromQuery] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsRequest request,
        [FromServices] UpdateDepartmentLocationsHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, request);
        return await handler.Handle(command, cancellationToken);
    }
    
    [HttpPut("{departmentId:Guid}/parent")]
    [ProducesResponseType<Envelope<Guid>>(201)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<Guid>> MoveDepartment(
        Guid departmentId,
        [FromBody] MoveDepartmentRequest request,
        [FromServices] MoveDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new MoveDepartmentCommand(departmentId, request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet("top-positions")]
    [ProducesResponseType<Envelope<GetDepartmentsWithTopPositionsResponse>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<GetDepartmentsWithTopPositionsResponse>> GetTopPositions(
        [FromServices] GetDepartmentsWithTopPositionsHandler handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(cancellationToken);
    }
    
    [HttpGet("roots")]
    [ProducesResponseType<Envelope<GetDepartmentsWithChildrenResponse>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<GetDepartmentsWithChildrenResponse>> GetDepartmentsWithChildren(
        [FromQuery] GetDepartmentsWithChildrenRequest request,
        [FromServices] GetDepartmentsWithChildrenHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new GetDepartmentsWithChildrenQuery(request);
        return await handler.Handle(command, cancellationToken);
    }
    
    [HttpGet("{parentId:guid}/children")]
    [ProducesResponseType<Envelope<GetChildrenByParentResponse>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(500)]
    public async Task<EndpointResult<GetChildrenByParentResponse>> GetChildrenByParent(
        Guid parentId,
        [FromQuery] GetChildrenByParentRequest request,
        [FromServices] GetChildrenByParentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new GetChildrenByParentQuery(parentId, request);
        return await handler.Handle(command, cancellationToken);
    }
}