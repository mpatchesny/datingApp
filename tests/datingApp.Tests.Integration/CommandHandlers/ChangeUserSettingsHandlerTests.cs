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

public class ChangeUserSettingsHandlerTests : IDisposable
{
    [Fact]
    public async Task change_existing_user_settings_should_succeed()
    {
        var command = new ChangeUserSettings(1, 18, 20, 20, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_nonexisting_user_settings_should_throw_exception()
    {
        var command = new ChangeUserSettings(2, 18, 20, 20, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly ChangeUserSettingsHandler _handler;
    private readonly TestDatabase _testDb;
    public ChangeUserSettingsHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _handler = new ChangeUserSettingsHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}