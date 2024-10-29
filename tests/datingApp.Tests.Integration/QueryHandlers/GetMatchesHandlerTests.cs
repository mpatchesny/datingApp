using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMatchesHandlerTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_GetMatchesHandler_by_user_id_should_return_nonempty_collection_of_matches_dto()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var query = new GetMatches() { UserId = user1.Id };
        var matches = await _handler.HandleAsync(query);
        Assert.NotEmpty(matches.Data);
        Assert.IsType<MatchDto>(matches.Data.First());
    }

    [Fact]
    public async Task given_user_not_exists_GetMatchesHandler_by_user_id_throws_UserNotExistsException()
    {
        var query = new GetMatches() { UserId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task returned_matches_count_is_lower_or_equal_to_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(5);
        var matches = await _handler.HandleAsync(query);
        Assert.InRange(matches.Data.Count(), 0, query.PageSize);
    }

    [Fact]
    public async Task given_page_above_1_returns_proper_number_of_matches()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var matchesToCreate = 9;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(5);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);
        Assert.NotEmpty(matches.Data);
        Assert.Equal(4, matches.Data.Count());
    }

    [Fact]
    public async Task returned_match_dto_user_is_not_the_user_who_make_request()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var query = new GetMatches() { UserId = user1.Id };
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(user2.Id.Value, matches.Data.First().User.Id);
    }

    [Fact]
    public async Task given_match_is_displayed_GetMatches_returns_is_displayed_by_the_user_who_make_request()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id, true);

        var query = new GetMatches() { UserId = user1.Id };
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(matches.Data.First().IsDisplayed, true);
    }
    
    [Fact]
    public async Task paginated_data_dto_returns_proper_number_page_count()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var matchesToCreate = 9;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(9, matches.PageCount);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_number_of_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var matchesToCreate = 15;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(9);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(9, matches.PageSize);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_testDb);
            _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, tempUser.Id);
        }

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(2, matches.Page);
    }
    
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMatchesHandler _handler;
    public GetMatchesHandlerTests()
    {
        _testDb = new TestDatabase();
        _handler = new GetMatchesHandler(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}