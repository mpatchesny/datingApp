using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Api.Controllers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using FluentStorage.Utils.Extensions;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMatchesHandlerTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_GetMatchesHandler_should_return_nonempty_collection_of_matches_dto_ordered_by_created_date_desc_with_one_last_message()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var createdTime = DateTime.UtcNow;
        var matches = new List<Match>();
        for (int i = 0; i < 10; i++)
        {
            var messages = new List<Message>();
            for (int j = 0; j < 10; j++)
            {
                messages.Add(new Message(Guid.NewGuid(), user2.Id, "hello", false, createdTime.AddSeconds(-j)));
            }
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages, createdAt: createdTime.AddSeconds(-i));
            matches.Add(match);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        var retrievedMatches = await _handler.HandleAsync(query);

        Assert.NotEmpty(retrievedMatches.Data);
        for (int i = 0; i < retrievedMatches.Data.Count; i++)
        {
            var matchDto = retrievedMatches.Data[i];
            Assert.NotNull(matchDto);
            Assert.IsType<MatchDto>(matchDto);
            Assert.Equal(matches[i].Id.Value, matchDto.Id);
            Assert.InRange(matchDto.Messages.Count(), 0, 1);
            Assert.Equal(RoundToMillisecond(createdTime), RoundToMillisecond(matchDto.Messages.ElementAt(0).CreatedAt));
        }
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
    public async Task returned_matches_count_is_equal_to_page_size_if_enough_matches()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(5);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(query.PageSize, matches.Data.Count);
    }

    [Fact]
    public async Task returned_matches_count_is_less_than_page_size_if_not_enough_matches()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 4;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(5);
        var matches = await _handler.HandleAsync(query);

        Assert.InRange(matches.Data.Count, 0, query.PageSize);
    }

    [Fact]
    public async Task given_page_above_1_GetMatches_returns_proper_number_of_matches()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 9;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(user2.Id.Value, matches.Data.First().User.Id);
    }

    [Fact]
    public async Task given_match_is_displayed_GetMatches_returns_is_displayed_by_the_user_who_make_request()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, isDisplayedByUser1: true);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(true, matches.Data.First().IsDisplayed);
    }

    [Fact]
    public async Task GetMatches_returns_proper_page_count()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 9;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(9, matches.PageCount);
    }

    [Fact]
    public async Task GetMatches_returns_proper_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 15;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(9);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(9, matches.PageSize);
    }

    [Fact]
    public async Task GetMatches_returns_proper_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(2, matches.Page);
    }

    [Fact]
    public async Task given_page_exceeds_matches_count_GetMatches_returns_empty_list()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatches() { UserId = user1.Id };
        query.SetPageSize(5);
        query.SetPage(3);
        var matches = await _handler.HandleAsync(query);

        Assert.Empty(matches.Data);
    }
    
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<ISpatial> _mockedSpatial;
    private readonly GetMatchesHandler _handler;
    public GetMatchesHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _mockedSpatial = new Mock<ISpatial>();
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0);
        _handler = new GetMatchesHandler(_testDb.ReadOnlyDbContext, _mockedSpatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }

    private static DateTime RoundToMillisecond(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
    }
}