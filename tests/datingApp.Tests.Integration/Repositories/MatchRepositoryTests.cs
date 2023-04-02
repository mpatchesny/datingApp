using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

public class MatchRepositoryTests : IDisposable
{
    [Fact]
    public async void get_existing_match_by_user_id_should_return_only_users_matches_and_match_messages()
    {
        var userId = 1;
        var matches = await _repository.GetByUserIdAsync(userId);
        Assert.Equal(1, matches.Count());
        Assert.Equal(0, matches.Where(x => x.UserId1 != userId && x.UserId2 != userId).Count());
    }

    [Fact]
    public async void get_match_by_nonexisting_user_id_should_return_empty_collection()
    {
        var matches = await _repository.GetByUserIdAsync(0);
        Assert.Equal(0, matches.Count());
    }

    [Fact]
    public async void delete_existing_match_by_id_should_succeed()
    {
        var matchId = 1;
        await _repository.DeleteAsync(matchId);
        _testDb.DbContext.SaveChanges();
        var matches = await _repository.GetByUserIdAsync(1);
        Assert.Equal(0, matches.Count());
    }

    [Fact]
    public async void delete_nonexisting_match_by_id_should_throw_exception()
    {
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(2));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void add_match_should_succeed()
    {
        var match = new Match(0, 1, 1, null, DateTime.UtcNow);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.Null(exception);
    }

    [Fact]
    public async void after_add_match_get_matches_should_return_plus_one_elements()
    {
        var match = new Match(0, 1, 1, null, DateTime.UtcNow);
        await _repository.AddAsync(match);
        _testDb.DbContext.SaveChanges();
        var matches = await _repository.GetByUserIdAsync(1);
        Assert.Equal(2, matches.Count());
    }

    // Arrange
    private readonly IMatchRepository _repository;
    private readonly TestDatabase _testDb;
    public MatchRepositoryTests()
    {
        _testDb = new TestDatabase();
        
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456799", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var settings2 = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(0, "123456788", "test2@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings2);
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var match = new Match(0, 1, 1, new List<Message>{ new Message(0, 1, 1, "match 1", false, DateTime.UtcNow) }, DateTime.UtcNow);
        var match2 = new Match(0, 2, 2, new List<Message>{ new Message(0, 2, 2, "match 2", false, DateTime.UtcNow) }, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.Matches.Add(match2);
        _testDb.DbContext.SaveChanges();
        _repository = new PostgresMatchRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}