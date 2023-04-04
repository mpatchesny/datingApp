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

public class ChangeUserHandlerTests : IDisposable
{
    [Fact]
    public async Task change_existing_user_should_succeed()
    {
        var command = new ChangeUser(1, new DateOnly(1998, 1, 1));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_nothing_should_succeed()
    {
        var command = new ChangeUser(1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_existing_user_without_changing_date_of_birth_should_succeed()
    {
        var command = new ChangeUser(1, null, "new bio");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_nonexisting_user_should_throw_exception()
    {
        var command = new ChangeUser(2, new DateOnly(1998, 1, 1));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly ChangeUserHandler _handler;
    private readonly TestDatabase _testDb;
    public ChangeUserHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _handler = new ChangeUserHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}