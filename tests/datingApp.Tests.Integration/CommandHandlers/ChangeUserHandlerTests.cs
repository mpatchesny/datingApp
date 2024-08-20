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
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), "1998-01-01");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_nothing_should_succeed()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_existing_user_without_changing_date_of_birth_should_succeed()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), null, "new bio");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_nonexisting_user_should_throw_exception()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000002"), "1998-01-01");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_invalid_date_change_user_should_throw_exception()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), "01.01.1998");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthFormatException>(exception);
    }

    [Fact]
    public async Task change_existing_user_settings_should_succeed()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), DiscoverAgeFrom: 18, DiscoverAgeTo: 20, DiscoverRange: 20, DiscoverSex: 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_location_of_existing_user_should_succeed()
    {
        var command = new ChangeUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), Lat: 40.0, Lon: 40.0);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    // Arrange
    private readonly ChangeUserHandler _handler;
    private readonly TestDatabase _testDb;
    public ChangeUserHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var userRepository = new DbUserRepository(_testDb.DbContext);
        _handler = new ChangeUserHandler(userRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}