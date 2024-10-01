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
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMessagesHandlerTests : IDisposable
{
    [Fact]
    public async Task given_match_exists_get_messages_by_match_id_returns_nonempty_collection_of_messages_dto()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 5; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id};
        var messages = await _handler.HandleAsync(query);
        Assert.NotEmpty(messages.Data);
        Assert.IsType<MessageDto>(messages.Data.First());
    }

    [Fact]
    public async Task given_authorization_serivce_returns_fail_GetMessages_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);

        var query = new GetMessages() { MatchId = match.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_not_exists_get_messages_by_match_id_returns_MatchNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var query = new GetMessages() { MatchId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task returned_messages_count_is_lower_or_equal_to_page_size()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 10; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        var messages = await _handler.HandleAsync(query);
        Assert.InRange(messages.Data.Count(), 0, query.PageSize);
    }

    [Fact]
    public async Task given_page_above_1_proper_number_of_messages_are()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(5);
        query.SetPage(2);
        var messages = await _handler.HandleAsync(query);
        Assert.NotEmpty(messages.Data);
        Assert.Equal(4, messages.Data.Count());
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_number_page_count()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var messages = await _handler.HandleAsync(query);
        Assert.Equal(9, messages.PageCount);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_number_of_page_size()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(1);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(1, matches.PageSize);
    }

    [Fact]
    public async Task paginated_data_dto_returns_proper_page()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i = 0; i < 9; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user1.Id);
        }

        var query = new GetMessages() { MatchId = match.Id };
        query.SetPageSize(1);
        query.SetPage(2);
        var matches = await _handler.HandleAsync(query);
        Assert.Equal(2, matches.Page);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMessagesHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetMessagesHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetMessagesHandler(_testDb.DbContext, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}