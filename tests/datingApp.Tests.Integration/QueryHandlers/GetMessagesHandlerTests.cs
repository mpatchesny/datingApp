using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMessagesHandlerTests : IDisposable
{
    [Fact]
    public async Task given_match_exists_GetMessagesHandler_returns_nonempty_collection_of_lastest_messages_dto_ordered_by_creation_time_desc()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var createdTime = DateTime.UtcNow;
        var messages = new List<Message>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id, createdAt: createdTime.AddSeconds(i)));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id};
        query.SetPageSize(5);
        var retrievedMessages = await _handler.HandleAsync(query);

        Assert.NotEmpty(retrievedMessages.Data);
        Assert.IsType<MessageDto>(retrievedMessages.Data.First());
        Assert.Collection(retrievedMessages.Data,
            m => Assert.Equal(messages[^1].Id.Value, m.Id),
            m => Assert.Equal(messages[^2].Id.Value, m.Id),
            m => Assert.Equal(messages[^3].Id.Value, m.Id),
            m => Assert.Equal(messages[^4].Id.Value, m.Id),
            m => Assert.Equal(messages[^5].Id.Value, m.Id));
    }

    [Fact]
    public async Task given_authorization_failed_GetMessagesHandler_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>() { IntegrationTestHelper.CreateMessage(user1.Id) };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_not_exists_GetMessagesHandler_returns_MatchNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));

        var query = new GetMessages() { MatchId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task GetMessagesHandler_returned_messages_count_is_lower_or_equal_to_page_size()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        var retrievedMessages = await _handler.HandleAsync(query);

        Assert.InRange(retrievedMessages.Data.Count, 0, query.PageSize);
    }

    [Fact]
    public async Task given_page_above_1_GetMessagesHandler_returns_proper_number_of_messages()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        query.SetPage(2);
        var retrievedMessages = await _handler.HandleAsync(query);

        Assert.NotEmpty(retrievedMessages.Data);
        Assert.Equal(4, retrievedMessages.Data.Count);
    }

    [Fact]
    public async Task GetMessagesHandler_returns_proper_page_count()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var retrievedMessages = await _handler.HandleAsync(query);

        Assert.Equal(9, retrievedMessages.PageCount);
    }

    [Fact]
    public async Task GetMessagesHandler_returns_proper_page_size()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(1, matches.PageSize);
    }

    [Fact]
    public async Task GetMessagesHandler_returns_proper_page()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);

        Assert.Equal(2, matches.Page);
    }

    [Fact]
    public async Task given_page_exceeds_messages_count_GetMessagesHandler_empty_list()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i = 0; i < 9; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user1.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        query.SetPage(3);
        var matches = await _handler.HandleAsync(query);

        Assert.Empty(matches.Data);
    }

    [Fact]
    public async Task GetMessagesHandler_returns_newest_messages_first()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var createdAt = DateTime.UtcNow;
        var messages = new List<Message>();
        for (int i = 0; i < 15; i++)
        {
            var message = IntegrationTestHelper.CreateMessage(user1.Id, createdAt: createdAt.AddSeconds(i));
            messages.Add(message);
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMessages() { MatchId = match.Id };
        var retrievedMessages = await _handler.HandleAsync(query);

        Assert.Equal(messages.OrderByDescending(m => m.CreatedAt).Select(m => m.Id.Value), retrievedMessages.Data.Select(m => m.Id));
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly GetMessagesHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetMessagesHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetMessagesHandler(_testDb.ReadOnlyDbContext, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}