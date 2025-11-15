using DirectoryService.Application.Departments.Commands.Create;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Request;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : DirectoryServiceBaseTests
{
    public CreateDepartmentTests(DirectoryServiceTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateDepartmentParent_with_valid_data_should_succeed()
    {
        // arrange
        var locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest("HQ", "hq", [locationId.Value]));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);

            Assert.Equal(department.Id.Value, result.Value);

            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartmentChildren_with_valid_data_should_succeed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentRequest("HQ", "hq", [locationId.Value], parentId.Value));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);

            Assert.Equal(department.Id.Value, result.Value);

            Assert.Equal(1, department.Depth);

            Assert.True(department.Path.Value != department.Identifier.Value);

            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_without_locations_should_failed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentRequest("HQ", "hq", []));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Error);
    }

    private async Task<DepartmentId> CreateParentDepartment(LocationId locationId)
    {
        var departmentId = new DepartmentId(Guid.NewGuid());

        var departmentLocations = new List<DepartmentLocation>
        {
            DepartmentLocation.Create(departmentId, locationId).Value,
        };

        return await ExecuteInDb(async dbContext =>
        {
            var department = Department.CreateParent(
                DepartmentName.Create("HQ").Value,
                DepartmentIdentifier.Create("hq").Value,
                departmentLocations).Value;

            dbContext.Departments.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }

    private async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Location.Create(
                LocationName.Create("Локация").Value,
                LocationAddress.Create("Страна", "Город", "Улица").Value,
                LocationTimezone.Create("Europe/Moscow").Value).Value;

            dbContext.Locations.Add(location);

            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartmentHandler, Task<T>> action)
    {
        var scope = Services.CreateAsyncScope();

        var sut = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();

        return await action(sut);
    }
}