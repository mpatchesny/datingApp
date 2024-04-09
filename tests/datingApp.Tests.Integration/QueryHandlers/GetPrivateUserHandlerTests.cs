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


public class GetPrivateUserHandlerTests : IDisposable
{
    [Fact]
    public async Task query_existing_user_should_return_private_user_dto()
    {
        var query = new GetPrivateUser();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var user = await _handler.HandleAsync(query);
        Assert.NotNull(user);
        Assert.IsType<PrivateUserDto>(user);
    }

    [Fact]
    public async Task query_nonexisting_user_should_return_null()
    {
        var query = new GetPrivateUser();
        query.UserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var user = await _handler.HandleAsync(query);
        Assert.Null(user);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetPrivateUserHandler _handler;
    public GetPrivateUserHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
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