using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

public class GetPrivateUserHandlerTests
{
    [Fact]
    public async Task query_existing_user_should_return_public_user_dto()
    {
        var query = new GetPrivateUser();
        query.UserId = 1;
        var user = await _handler.HandleAsync(query);
        Assert.NotNull(user);
        Assert.IsType<PrivateUserDto>(user);
    }

    [Fact]
    public async Task query_nonexisting_user_should_return_null()
    {
        var query = new GetPrivateUser();
        query.UserId = 2;
        var user = await _handler.HandleAsync(query);
        Assert.Null(user);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPrivateUserHandler _handler;
    public GetPrivateUserHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(0, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _handler = new GetPrivateUserHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}