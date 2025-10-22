using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.DepartmentPositions;

public sealed class DepartmentPosition
{
    public DepartmentPositionId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public PositionId PositionId { get; init; }
}