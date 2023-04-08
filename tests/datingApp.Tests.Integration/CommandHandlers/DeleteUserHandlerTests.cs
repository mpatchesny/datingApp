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

public class DeleteUserHandlerTests : IDisposable
{
    [Fact]
    public async void delete_existing_user_should_succeed()
    {
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new DeleteUser(1)));
        Assert.Null(exception);
    }

    [Fact]
    public async void delete_nonexisting_user_should_throw_exception()
    {
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new DeleteUser(2)));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly DeleteUserHandler _handler;
    private readonly TestDatabase _testDb;
    public DeleteUserHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _handler = new DeleteUserHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}