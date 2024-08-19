using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.Spatial;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetPublicUserHanlderTests : IDisposable
{
    [Fact]
    public async Task query_existing_user_should_return_public_user_dto()
    {
        var query = new GetPublicUser();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.UserRequestedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var user = await _handler.HandleAsync(query);
        Assert.NotNull(user);
        Assert.IsType<PublicUserDto>(user);
    }

    [Fact]
    public async Task query_nonexisting_user_should_user_not_exists_exception()
    {
        var query = new GetPublicUser();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        query.UserRequestedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task request_by_nonexisting_user_should_return_null()
    {
        var query = new GetPublicUser();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        query.UserRequestedId = Guid.Parse("00000000-0000-0000-0000-000000000005");
        var user = await _handler.HandleAsync(query);
        Assert.Null(user);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPublicUserHandler _handler;
    public GetPublicUserHanlderTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        
        var mockedSpatial = new Mock<ISpatial>();
        mockedSpatial.Setup(m => m.CalculateDistance(0.0, 0.0, 0.0, 0.0)).Returns(0);
        _handler = new GetPublicUserHandler(_testDb.DbContext, mockedSpatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}