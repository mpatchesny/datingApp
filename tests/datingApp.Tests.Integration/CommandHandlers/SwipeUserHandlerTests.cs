using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class SwipeUserHandlerTests : IDisposable
{
    [Fact]
    public async Task add_swipe_should_succeed()
    {
        var command = new SwipeUser(1, 2, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task add_swipe_for_nonexistsing_user_should_throw_exception()
    {
        var command = new SwipeUser(1, 3, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly SwipeUserHandler _handler;
    private readonly TestDatabase _testDb;
    public SwipeUserHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(0, "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var swipeRepository = new PostgresSwipeRepository(_testDb.DbContext);
        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _handler = new SwipeUserHandler(swipeRepository, userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}