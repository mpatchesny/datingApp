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
        Assert.NotEmpty(matches);
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

    [Fact]
    public async Task returned_matches_count_is_lower_or_equal_to_page_size()
    {
        var query = new GetMatches();
        query.UserId = 1;
        query.PageSize = 5;
        var matches = await _handler.HandleAsync(query);
        Assert.InRange(matches.Count(), 0, query.PageSize);
    }

    [Fact]
    public async Task returns_proper_number_of_messages_when_page_above_1()
    {
        var query = new GetMatches();
        query.UserId = 1;
        query.PageSize = 5;
        query.Page = 2;
        var matches = await _handler.HandleAsync(query);
        Assert.NotEmpty(matches);
        Assert.Equal(4, matches.Count());
    }

    [Fact]
    public async Task returned_match_dto_user_is_not_the_user_who_make_request()
    {
        var query = new GetMatches();
        query.UserId = 1;
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(2, matches.First().UserId);
    }

    [Fact]
    public async Task returned_match_dto_is_displayed_returns_is_displayed_by_the_user_who_make_request()
    {
        var query = new GetMatches();
        query.UserId = 1;
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(matches.First().IsDisplayed, true);
    }
    
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMatchesHandler _handler;
    public GetMatchesHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 21, 20, 45.5, 45.5);
        var user = new User(0, "111111111", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var settings2 = new UserSettings(0, Sex.Male, 18, 21, 20, 45.5, 45.5);
        var user2 = new User(0, "222222222", "test2@test.com", "Karyna", new DateOnly(2000,1,1), Sex.Female, null, settings);
        var matches = new List<Core.Entities.Match>
        {
            new Core.Entities.Match(0, 1, 2, true, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow),
            new Core.Entities.Match(0, 1, 1, false, false, null, DateTime.UtcNow)
        };
        
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.Users.Add(user2);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Matches.AddRange(matches);
        _testDb.DbContext.SaveChanges();
        _handler = new GetMatchesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}