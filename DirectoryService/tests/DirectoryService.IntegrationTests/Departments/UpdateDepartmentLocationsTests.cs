using DirectoryService.Application.Departments.Commands.UpdateLocations;
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

public class UpdateDepartmentLocationsTests : DirectoryServiceBaseTests
{
    public UpdateDepartmentLocationsTests(DirectoryServiceTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateDepartmentLocation_with_valid_data_should_succeed()
    {
        // arrange
        var locationOneId = await CreateLocation("Локация 1", "Улица 1");

        var departmentId = await CreateDepartment(locationOneId);

        var locationTwoId = await CreateLocation("Локация 2", "Улица 2");

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest([locationTwoId.Value]));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var departmentLocations = await dbContext.DepartmentLocations
                .Where(dl => dl.DepartmentId == departmentId)
                .ToListAsync(cancellationToken);

            Assert.NotNull(departmentLocations);

            Assert.True(result.IsSuccess);

            Assert.Contains(locationTwoId, departmentLocations.Select(dl => dl.LocationId));
        });
    }

    [Fact]
    public async Task UpdateDepartmentLocation_with_two_locations_should_succeed()
    {
        // arrange
        var locationOneId = await CreateLocation("Локация 1", "Улица 1");

        var departmentId = await CreateDepartment(locationOneId);

        var newLocationsIdList = new List<LocationId>
        {
            await CreateLocation("Локация 3", "Улица 3"), await CreateLocation("Локация 4", "Улица 3"),
        };

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest(newLocationsIdList.Select(l => l.Value)));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var departmentLocations = await dbContext.DepartmentLocations
                .Where(dl => dl.DepartmentId == departmentId)
                .ToListAsync(cancellationToken);

            Assert.NotNull(departmentLocations);

            Assert.Equal(2, departmentLocations.Count);

            Assert.True(result.IsSuccess);

            Assert.Equal(
                newLocationsIdList.Select(l => l.Value).OrderBy(l => l),
                departmentLocations.Select(dl => dl.LocationId.Value).OrderBy(l => l));
        });
    }

    [Fact]
    public async Task UpdateDepartmentLocation_without_locations_should_failed()
    {
        // arrange
        var locationId = await CreateLocation("Локация 1", "Улица 1");

        var departmentId = await CreateDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest([]));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Error);
    }
    
    [Fact]
    public async Task UpdateDepartmentLocation_with_no_exist_locations_should_failed()
    {
        // arrange
        var locationId = await CreateLocation("Локация 1", "Улица 1");

        var departmentId = await CreateDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest([Guid.NewGuid()]));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Error);
    }

    private async Task<DepartmentId> CreateDepartment(LocationId locationId)
    {
        return await ExecuteInDb(async dbContext =>
        {
            var departmentId = new DepartmentId(Guid.NewGuid());

            var departmentLocations = new List<DepartmentLocation>
            {
                DepartmentLocation.Create(departmentId, locationId).Value,
            };

            var department = Department.CreateParent(
                DepartmentName.Create("HQ").Value,
                DepartmentIdentifier.Create("hq").Value,
                departmentLocations).Value;

            dbContext.Departments.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }

    private async Task<LocationId> CreateLocation(string locationName, string streetName)
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Location.Create(
                LocationName.Create(locationName).Value,
                LocationAddress.Create("Страна", "Город", streetName).Value,
                LocationTimezone.Create("Europe/Moscow").Value).Value;

            dbContext.Locations.Add(location);

            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<UpdateDepartmentLocationsHandler, Task<T>> action)
    {
        var scope = Services.CreateAsyncScope();

        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentLocationsHandler>();

        return await action(sut);
    }
}