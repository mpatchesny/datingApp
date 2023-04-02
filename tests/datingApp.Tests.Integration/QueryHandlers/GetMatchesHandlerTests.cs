using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;

[Collection("Integration tests")]
public class GetMatchesHandlerTests
{
    [Fact]
    public async Task query_matches_by_existing_user_id_should_return_nonempty_collection_of_matches_dto()
    {
        var query = new GetMatches();
        query.UserId = 1;
        var matches = await _handler.HandleAsync(query);
        Assert.Single(matches);
        Assert.IsType<MatchDto>(matches.First());
    }

    [Fact]
    public async Task query_matches_by_nonexisting_user_id_should_return_empty_collection()
    {
        var query = new GetMatches();
        query.UserId = 0;
        var matches = await _handler.HandleAsync(query);
        Assert.Empty(matches);
    }
    
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMatchesHandler _handler;
    public GetMatchesHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(0, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var match = new Core.Entities.Match(0, 1, 1, null, DateTime.UtcNow);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.Add(match);
        _testDb.DbContext.SaveChanges();
        _handler = new GetMatchesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}