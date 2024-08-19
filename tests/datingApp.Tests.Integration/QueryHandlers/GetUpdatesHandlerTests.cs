using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetUpdatesHandlerTests : IDisposable
{
    [Fact]
    public async Task get_updates_should_return_matches_for_given_user()
    {
        var users = new List<User>();
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test1@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111112", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "111111113", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "111111114", "test4@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000005"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "111111115", "test5@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        _testDb.DbContext.Users.AddRange(users);
        await _testDb.DbContext.SaveChangesAsync();

        var matches = new List<Match> {
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), false, false, null, DateTime.UtcNow),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000005"), false, false, null, DateTime.UtcNow),
        };

        _testDb.DbContext.Matches.AddRange(matches);
        await _testDb.DbContext.SaveChangesAsync();

        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var query = new GetUpdates{ UserId = userId, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task get_updates_should_return_matches_after_last_activity_time()
    {
        var users = new List<User>();
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test1@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111112", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "111111113", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "111111114", "test4@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000005"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "111111115", "test5@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        _testDb.DbContext.Users.AddRange(users);
        await _testDb.DbContext.SaveChangesAsync();

        var matches = new List<Match> {
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000003"), false, false, null, DateTime.UtcNow - TimeSpan.FromHours(1))
        };

        _testDb.DbContext.Matches.AddRange(matches);
        await _testDb.DbContext.SaveChangesAsync();

        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var query = new GetUpdates{ UserId = userId, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task get_updates_should_return_matches_that_have_messages_send_after_last_activity_time()
    {
        var users = new List<User>();
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test1@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111112", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "111111113", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "111111114", "test4@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000005"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "111111115", "test5@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        _testDb.DbContext.Users.AddRange(users);
        await _testDb.DbContext.SaveChangesAsync();

        var matches = new List<Match> {
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow - TimeSpan.FromHours(1)),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000003"), false, false, null, DateTime.UtcNow - TimeSpan.FromHours(1))
        };

        _testDb.DbContext.Matches.AddRange(matches);
        await _testDb.DbContext.SaveChangesAsync();

        var messages = new List<Message> {
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", false, DateTime.UtcNow + TimeSpan.FromHours(1)),
            new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", false, DateTime.UtcNow + TimeSpan.FromHours(1))
        };

        _testDb.DbContext.Messages.AddRange(messages);
        await _testDb.DbContext.SaveChangesAsync();

        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var query = new GetUpdates{ UserId = userId, LastActivityTime = DateTime.UtcNow};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task when_no_matches_match_criteria_get_updates_should_return_empty_collection()
    {
        var users = new List<User>();
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "111111111", "test1@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111112", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000003"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "111111113", "test3@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));

        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000004"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "111111114", "test4@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000005"), Sex.Female, 18, 21, 20, 45.5, 45.5);
        users.Add(new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "111111115", "test5@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings));
        
        _testDb.DbContext.Users.AddRange(users);
        await _testDb.DbContext.SaveChangesAsync();

        var matches = new List<Match> {
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow - TimeSpan.FromMinutes(1)),
            new Match(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000004"), false, false, null, DateTime.UtcNow + TimeSpan.FromMinutes(1)),
        };

        _testDb.DbContext.Matches.AddRange(matches);
        await _testDb.DbContext.SaveChangesAsync();

        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var query = new GetUpdates{ UserId = userId, LastActivityTime = DateTime.UtcNow};
        var result = await _handler.HandleAsync(query);
        Assert.Empty(result);
    }

    [Fact]
    public async Task given_user_who_requestes_get_updates_not_exists_get_updates_returns_user_not_exists_exception()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var query = new GetUpdates{ UserId = userId, LastActivityTime = DateTime.UtcNow};
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetUpdatesHandler _handler;
    public GetUpdatesHandlerTests()
    {
        _testDb = new TestDatabase();
        _handler = new GetUpdatesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}