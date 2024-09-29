using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class MatchRepositoryTests : IDisposable
{
    [Fact]
    public async void given_match_exists_get_match_by_user_id_should_return_only_users_matches_and_match_messages()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var matches = await _repository.GetByUserIdAsync(userId);
        Assert.Single(matches);
        Assert.Empty(matches.Where(x => x.UserId1 != userId && x.UserId2 != userId));
    }

    [Fact]
    public async void get_match_by_nonexisting_user_id_should_return_empty_collection()
    {
        var matches = await _repository.GetByUserIdAsync(Guid.NewGuid());
        Assert.Empty(matches);
    }

    [Fact]
    public async void given_match_exists_get_match_by_id_should_succeed()
    {
        var match = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.NotNull(match);
    }

    [Fact]
    public async void given_match_not_exists_get_match_by_id_should_return_null()
    {
        var match = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(match);
    }

    [Fact]
    public async void given_match_exists_get_exsits_should_return_true()
    {
        var exists = await _repository.ExistsAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.True(exists);
    }
    [Fact]
    public async void given_match_not_exists_get_exsits_should_return_false()
    {
        var exists = await _repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.False(exists);
    }

    [Fact]
    public async void delete_existing_match_by_id_should_succeed()
    {
        var matchId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var match = await _repository.GetByIdAsync(matchId);
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(match));
        Assert.Null(exception);
        var deletedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Null(deletedMatch);
    }

    [Fact]
    public async void after_delete_match_get_matches_by_user_id_should_return_minus_one_elements()
    {
        var matchId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var match = await _repository.GetByIdAsync(matchId);
        await _repository.DeleteAsync(match);
        var matches = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Empty(matches);
    }

    [Fact]
    public async void add_match_should_succeed()
    {
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.Null(exception);
        var addedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Same(addedMatch, match);
    }

    [Fact]
    public async void update_match_should_succeed()
    {
        var match = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        match.SetDisplayed(match.UserId1);

        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(match));
        Assert.Null(exception);
        var updatedMatch = await _testDb.DbContext.Matches.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Same(updatedMatch, match);
    }

    [Fact]
    public async void add_match_with_existing_id_should_throw_InvalidOperationException()
    {
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);

        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(match));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void after_add_match_get_matches_by_user_id_should_return_plus_one_elements()
    {
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, null, DateTime.UtcNow);

        await _repository.AddAsync(match);
        var matches = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Equal(2, matches.Count());
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IMatchRepository _repository;
    public MatchRepositoryTests()
    {
        _testDb = new TestDatabase();
        
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), PreferredSex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456799", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);
        var settings2 = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000002"), PreferredSex.Female, 18, 20, 50, 40.5, 40.5);
        var user2 = new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "123456788", "test2@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings2);
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();

        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), false, false, new List<Message>{ new Message(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "match 1", false, DateTime.UtcNow) }, DateTime.UtcNow);
        var match2 = new Match(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, new List<Message>{ new Message(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "match 2", false, DateTime.UtcNow) }, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.Matches.Add(match2);
        _testDb.DbContext.SaveChanges();
        _repository = new DbMatchRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}