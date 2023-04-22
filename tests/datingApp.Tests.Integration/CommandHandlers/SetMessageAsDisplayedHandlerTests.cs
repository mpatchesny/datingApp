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
public class SetMessageAsDisplayedHandlerTests : IDisposable
{
    [Fact]
    public async Task set_existsing_message_as_displayed_should_succeed()
    {
        var command = new SetMessagesAsDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task when_not_user_who_received_message_set_message_as_displayed_should_throw_exception()
    {
        var command = new SetMessagesAsDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserCannotSetMessageAsDisplayedException>(exception);
    }

    [Fact]
    public async Task set_nonexistsing_message_as_displayed_should_throw_exception()
    {
        var command = new SetMessagesAsDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MessageNotExistsException>(exception);
    }

    // Arrange
    private readonly SetMessageAsDisplayedHandler _handler;
    private readonly TestDatabase _testDb;
    public SetMessageAsDisplayedHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);
        var message = new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"),"hello",false, DateTime.UtcNow);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Messages.Add(message);
        _testDb.DbContext.SaveChanges();

        var messageRepository = new PostgresMessageRepository(_testDb.DbContext);
        _handler = new SetMessageAsDisplayedHandler(messageRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}