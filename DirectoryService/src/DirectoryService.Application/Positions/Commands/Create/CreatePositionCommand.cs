using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.Positions.Request;

namespace DirectoryService.Application.Positions.Commands.Create;

public record CreatePositionCommand(
    CreatePositionRequest Request) : ICommand;