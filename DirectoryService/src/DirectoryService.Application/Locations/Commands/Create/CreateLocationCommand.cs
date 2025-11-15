using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Locations.Request;

namespace DirectoryService.Application.Locations.Commands.Create;

public record CreateLocationCommand(
    CreateLocationRequest Request) : ICommand;