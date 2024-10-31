using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SetMessagesAsDisplayedHandlerTests : IDisposable
{
    [Fact]
    public async Task given_message_exists_set_messages_as_displayed_should_succeed()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        var message = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var command = new SetMessagesAsDisplayed(message.Id, user2.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_message_not_exists_set_messages_as_displayed_should_do_nothing()
    {
        var command = new SetMessagesAsDisplayed(Guid.NewGuid(), Guid.NewGuid());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    // Arrange
    private readonly SetMessagesAsDisplayedHandler _handler;
    private readonly TestDatabase _testDb;
    public SetMessagesAsDisplayedHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(40.5, 40.5));
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), PreferredSex.Female, new PreferredAge(18, 20), 50, new Location(40.5, 40.5));
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings2);

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

        var matchRepository = new DbMatchRepository(_testDb.DbContext);
        _handler = new SetMessagesAsDisplayedHandler(matchRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}