using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Request;

namespace DirectoryService.Application.Departments.Commands.Create;

public record CreateDepartmentCommand(
    CreateDepartmentRequest Request) : ICommand;