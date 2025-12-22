using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments.Request;

namespace DirectoryService.Application.Departments.Commands.Delete;

public record DeleteByIdCommand(DeleteByIdRequest Request) : ICommand;