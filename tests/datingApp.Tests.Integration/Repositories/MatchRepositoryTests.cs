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
    public async void get_existing_match_by_user_id_should_succeed()
    {
        var matches = await _repository.GetByUserIdAsync(1);
        Assert.Equal(1, matches.Count());
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
        await _repository.AddAsync(match);
        _testDb.DbContext.SaveChanges();
        var matches = await _repository.GetByUserIdAsync(1);
        Assert.NotNull(matches);
        Assert.Equal(2, matches.Count());
    }

    // Arrange
    private readonly IMatchRepository _repository;
    private readonly TestDatabase _testDb;
    public MatchRepositoryTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var match = new Match(0, 1, 1, null, DateTime.UtcNow);
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();
        _repository = new PostgresMatchRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}