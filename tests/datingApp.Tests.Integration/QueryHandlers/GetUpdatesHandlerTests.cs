using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using MailKit;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetUpdatesHandlerTests : IDisposable
{
    [Fact]
    public async Task GetUpdatesHandler_returns_matches_for_given_user()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, createdAt: DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user3.Id, createdAt: DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user2.Id, user3.Id, createdAt: DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user3.Id, user4.Id, createdAt: DateTime.UtcNow);

        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);

        Assert.NotEmpty(result.Data);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetUpdatesHandler_returns_matches_after_last_activity_time()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, createdAt: DateTime.UtcNow);
        var timeAfterLastActivityTime = DateTime.UtcNow - TimeSpan.FromHours(1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user3.Id, createdAt: timeAfterLastActivityTime);

        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);

        Assert.NotEmpty(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetUpdatesHandler_returns_matches_that_have_messages_send_after_last_activity_time()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        var timeBeforeLastActivityTime = DateTime.UtcNow - TimeSpan.FromHours(1);
        var timeAfterLastActivityTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        var messages1 = new List<Message>() { IntegrationTestHelper.CreateMessage(user2.Id, createdAt: timeAfterLastActivityTime) };
        var messages2 = new List<Message>() { IntegrationTestHelper.CreateMessage(user3.Id, createdAt: timeAfterLastActivityTime) };
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages1, createdAt: timeBeforeLastActivityTime);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user3.Id, messages: messages2, createdAt: timeBeforeLastActivityTime);

        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow };
        var result = await _handler.HandleAsync(query);

        Assert.NotEmpty(result.Data);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task given_no_matches_match_criteria_GetUpdatesHandler_returns_empty_collection()
    {
        var userWithoutMatch = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        var timeBeforeLastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user2.Id, user3.Id, createdAt: timeBeforeLastActivityTime);
        var timeAfterLastActivityTime = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user3.Id, user4.Id, createdAt: timeAfterLastActivityTime);

        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates{ UserId = userWithoutMatch.Id, LastActivityTime = DateTime.UtcNow};
        var result = await _handler.HandleAsync(query);

        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task given_user_not_exists_GetUpdatesHandler_returns_UserNotExistsException()
    {
        var query = new GetUpdates{ UserId = Guid.NewGuid(), LastActivityTime = DateTime.UtcNow};
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task GetUpdatesHandler_returns_proper_page_count()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 9;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(9, matches.PageCount);
    }

    [Fact]
    public async Task GetUpdatesHandler_returns_proper_page_size()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 15;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates() { UserId = user1.Id };
        query.SetPageSize(9);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(9, matches.PageSize);
    }

    [Fact]
    public async Task GetUpdatesHandler_returns_proper_page()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates() { UserId = user1.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(2, matches.Page);
    }

    [Fact]
    public async Task given_page_exceeds_matches_count_GetUpdatesHandler_returns_empty_list()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var matchesToCreate = 10;
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            _ = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates() { UserId = user1.Id };
        query.SetPageSize(5);
        query.SetPage(3);
        var matches = await _handler.HandleAsync(query);

        Assert.Empty(matches.Data);
    }

    [Fact (Skip = "matches/messages are not sorted in any way")]
    public async Task GetUpdatesHandler_returns_newest_updates_first()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        var createdAt = DateTime.UtcNow;
        var matchesToCreate = 10;
        var matches = new List<Match>();
        for (int i = 0; i < matchesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id, createdAt: createdAt.AddSeconds(i));
            matches.Add(match);
        }
        
        var messagesToCreate = 5;
        var messages = new List<Message>();
        for (int i = 0; i < messagesToCreate; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            var message = new Message(Guid.NewGuid(), tempUser.Id, "hello", false, createdAt: createdAt.AddSeconds(i));
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, tempUser.Id, messages: new List<Message>() { message });
            messages.Add(message);
            matches.Add(match);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetUpdates() { UserId = user1.Id };
        var items = await _handler.HandleAsync(query);

        Assert.Equal(matches.OrderByDescending(m => m.CreatedAt).Select(m => m.Id.Value), items.Data.Select(m => m.Id));
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<ISpatial> _mockedSpatial;
    private readonly GetUpdatesHandler _handler;
    public GetUpdatesHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _mockedSpatial = new Mock<ISpatial>();
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0);
        _handler = new GetUpdatesHandler(_testDb.ReadOnlyDbContext, _mockedSpatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}