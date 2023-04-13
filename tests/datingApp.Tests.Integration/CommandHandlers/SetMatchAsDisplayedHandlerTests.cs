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
public class SetMatchAsDisplayedHandlerTests : IDisposable
{
    [Fact]
    public async Task set_existsing_match_as_displayed_should_succeed()
    {
        var command = new SetMatchAsDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task set_nonexistsing_match_as_displayed_should_throw_exception()
    {
        var command = new SetMatchAsDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000000"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }
    
    // Arrange
    private readonly SetMatchAsDisplayedHandler _handler;
    private readonly TestDatabase _testDb;
    public SetMatchAsDisplayedHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();

        var matchRepository = new PostgresMatchRepository(_testDb.DbContext);
        _handler = new SetMatchAsDisplayedHandler(matchRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}