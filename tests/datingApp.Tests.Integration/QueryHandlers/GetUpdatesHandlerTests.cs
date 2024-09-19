using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id, DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user3.Id, DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user2.Id, user3.Id, DateTime.UtcNow);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user3.Id, user4.Id, DateTime.UtcNow);

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task get_updates_should_return_matches_after_last_activity_time()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id, DateTime.UtcNow);
        var timeAfterLastActivityTime = DateTime.UtcNow - TimeSpan.FromHours(1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user3.Id, timeAfterLastActivityTime);

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow - TimeSpan.FromMinutes(1)};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task get_updates_should_return_matches_that_have_messages_send_after_last_activity_time()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var timeBeforeLastActivityTime = DateTime.UtcNow - TimeSpan.FromHours(1);
        var match1 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id, timeBeforeLastActivityTime);
        var match2 = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user3.Id, timeBeforeLastActivityTime);

        var timeAfterLastActivityTime = DateTime.UtcNow + TimeSpan.FromHours(1);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match1.Id, user2.Id, "test", timeAfterLastActivityTime);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match2.Id, user3.Id, "test", timeAfterLastActivityTime);

        var query = new GetUpdates{ UserId = user1.Id, LastActivityTime = DateTime.UtcNow};
        var result = await _handler.HandleAsync(query);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task when_no_matches_match_criteria_get_updates_should_return_empty_collection()
    {
        var userWithNotMatch = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user3 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user4 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var time1 = DateTime.UtcNow - TimeSpan.FromMinutes(1);
        var time2 = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user2.Id, user3.Id, time1);
        _ = await IntegrationTestHelper.CreateMatchAsync(_testDb, user3.Id, user4.Id, time2);

        var query = new GetUpdates{ UserId = userWithNotMatch.Id, LastActivityTime = DateTime.UtcNow};
        var result = await _handler.HandleAsync(query);
        Assert.Empty(result);
    }

    [Fact]
    public async Task given_user_not_exists_get_updates_returns_user_not_exists_exception()
    {
        var query = new GetUpdates{ UserId = Guid.NewGuid(), LastActivityTime = DateTime.UtcNow};
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