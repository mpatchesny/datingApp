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