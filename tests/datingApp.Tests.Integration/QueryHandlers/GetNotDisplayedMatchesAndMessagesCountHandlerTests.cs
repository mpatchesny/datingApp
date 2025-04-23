using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Application.Queries;
using datingApp.Infrastructure.DAL.Handlers;
using Xunit;
using datingApp.Core.Entities;
using datingApp.Application.Exceptions;

namespace datingApp.Tests.Integration.QueryHandlers;

public class GetNotDisplayedMatchesAndMessagesCountHandlerTests : IDisposable
{
    [Fact]
    public async void given_user_not_exists_GetNotDisplayedMatchesAndMessagesCountHandler_raise_UserNotExistsException()
    {
        var query = new GetNotDisplayedMatchesAndMessages() { UserId = Guid.NewGuid() };
        var exception = await Assert.ThrowsAsync<UserNotExistsException>(async () => await _handler.HandleAsync(query));
    }

    [Fact]
    public async void given_user_has_no_matches_or_messages_GetNotDisplayedMatchesAndMessagesCountHandler_returns_0()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        var query = new GetNotDisplayedMatchesAndMessages() { UserId = user.Id };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.Equal(0, result.Item1);
        Assert.Equal(0, result.Item2);
    }

    [Fact]
    public async void given_this_then_that()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);

        for (int i = 0; i < 5; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user.Id, tempUser.Id);
        }

        for (int i = 0; i < 7; i++)
        {
            var tempUser = await IntegrationTestHelper.CreateUserAsync(_dbContext);
            var messages = new List<Message>() { IntegrationTestHelper.CreateMessage(tempUser.Id) };
            var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user.Id, tempUser.Id, isDisplayedByUser1: true, isDisplayedByUser2: true, messages: messages);
        }
        _dbContext.ChangeTracker.Clear();

        var query = new GetNotDisplayedMatchesAndMessages() { UserId = user.Id };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.Equal(5, result.Item1);
        Assert.Equal(7, result.Item2);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly GetNotDisplayedMatchesAndMessagesHandler _handler;
    public GetNotDisplayedMatchesAndMessagesCountHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _handler = new GetNotDisplayedMatchesAndMessagesHandler(_testDb.ReadOnlyDbContext);
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}