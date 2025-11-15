using DirectoryService.Application.Departments.Commands.Move;
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

public class MoveDepartmentTests : DirectoryServiceBaseTests
{
    public MoveDepartmentTests(DirectoryServiceTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MoveChildren_to_without_parent_should_succeed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;
        
        var parent = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.Departments.FirstAsync(d => d.Id == parentId, cancellationToken);
        });

        var childrenId = await CreateChildrenDepartment("Children", locationId, parent);

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new MoveDepartmentCommand(childrenId.Value, new MoveDepartmentRequest(null));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments.FirstAsync(d => d.Id == childrenId, cancellationToken);

            Assert.NotNull(department);

            Assert.Equal(department.Id, childrenId);
            
            Assert.Equal(0, department.Depth);

            Assert.True(department.Path.Value == department.Identifier.Value);

            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task MoveChildren_to_another_parent_should_succeed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentOneId = await CreateParentDepartment(locationId);
        
        var parentTwoId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;
        
        var parentOne = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.Departments.FirstAsync(d => d.Id == parentOneId, cancellationToken);
        });

        var childrenId = await CreateChildrenDepartment("Children", locationId, parentOne);

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new MoveDepartmentCommand(childrenId.Value, new MoveDepartmentRequest(parentTwoId.Value));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments.FirstAsync(d => d.Id == childrenId, cancellationToken);

            Assert.NotNull(department);

            Assert.Equal(department.Id, childrenId);
            
            Assert.Equal(1, department.Depth);

            Assert.True(department.Path.Value != department.Identifier.Value);

            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }
    
    [Fact]
    public async Task MoveChildren_with_children_to_without_parent_should_succeed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;
        
        var parentOne = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.Departments.FirstAsync(d => d.Id == parentId, cancellationToken);
        });

        var childrenOneId = await CreateChildrenDepartment("Children 1", locationId, parentOne);
        
        var childrenOne = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.Departments.FirstAsync(d => d.Id == childrenOneId, cancellationToken);
        });
        
        var childrenTwoId = await CreateChildrenDepartment("Children 2", locationId, childrenOne);

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new MoveDepartmentCommand(childrenOneId.Value, new MoveDepartmentRequest(null));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var childrenOneAfterMove = await dbContext.Departments
                .FirstAsync(d => d.Id == childrenOneId, cancellationToken);
            
            var childrenTwoAfterMove = await dbContext.Departments
                .FirstAsync(d => d.Id == childrenTwoId, cancellationToken);

            Assert.NotNull(childrenOneAfterMove);

            Assert.Equal(childrenOneAfterMove.Id, childrenOneId);
            
            Assert.Equal(0, childrenOneAfterMove.Depth);

            Assert.True(childrenOneAfterMove.Path.Value == childrenOneAfterMove.Identifier.Value);
            
            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value);
            
            Assert.NotNull(childrenTwoAfterMove);

            Assert.Equal(childrenTwoAfterMove.Id, childrenTwoId);
            
            Assert.Equal(1, childrenTwoAfterMove.Depth);

            Assert.NotEqual("parent.children.children", childrenTwoAfterMove.Path.Value);
        });
    }
    
    [Fact]
    public async Task MoveParent_to_children_should_failed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;
        
        var parent = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.Departments.FirstAsync(d => d.Id == parentId, cancellationToken);
        });

        var childrenId = await CreateChildrenDepartment("Children", locationId, parent);

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new MoveDepartmentCommand(parentId.Value, new MoveDepartmentRequest(childrenId.Value));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Error);
    }
    
    [Fact]
    public async Task MoveParent_to_yourself_should_failed()
    {
        // arrange
        var locationId = await CreateLocation();

        var parentId = await CreateParentDepartment(locationId);

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = new MoveDepartmentCommand(parentId.Value, new MoveDepartmentRequest(parentId.Value));

            return sut.Handle(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Error);
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
                DepartmentName.Create("Parent").Value,
                DepartmentIdentifier.Create("parent").Value,
                departmentLocations).Value;

            dbContext.Departments.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }

    private async Task<DepartmentId> CreateChildrenDepartment(
        string departmentName,
        LocationId locationId,
        Department parent)
    {
        var departmentId = new DepartmentId(Guid.NewGuid());

        var departmentLocations = new List<DepartmentLocation>
        {
            DepartmentLocation.Create(departmentId, locationId).Value,
        };

        return await ExecuteInDb(async dbContext =>
        {
            var department = Department.CreateChild(
                DepartmentName.Create(departmentName).Value,
                DepartmentIdentifier.Create("children").Value,
                parent,
                departmentLocations).Value;

            dbContext.Departments.Add(department);

            await dbContext.SaveChangesAsync();

            return department.Id;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<MoveDepartmentHandler, Task<T>> action)
    {
        var scope = Services.CreateAsyncScope();

        var sut = scope.ServiceProvider.GetRequiredService<MoveDepartmentHandler>();

        return await action(sut);
    }
}