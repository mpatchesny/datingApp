using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class DeleteMatchHandlerTests 
{
    [Fact]
    public async Task delete_existing_match_should_succeed()
    {
        var command = new DeleteMatch(1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }
    
    [Fact]
    public async Task delete_nonexisting_match_should_throw_exception()
    {
        var command = new DeleteMatch(1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }
    
    // Arrange
    private readonly DeleteMatchHandler _handler;
    private readonly TestDatabase _testDb;
    public DeleteMatchHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var settings2 = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(0, "111111111", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);

        var match = new Match(0, 1, 2, false, false, null, DateTime.UtcNow);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();

        var matchRepository = new PostgresMatchRepository(_testDb.DbContext);
        _handler = new DeleteMatchHandler(matchRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}