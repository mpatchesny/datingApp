using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMatchHandlerTests : IDisposable
{
    [Fact]
    public async Task given_match_exists_get_match_handler_should_return_match_dto_with_non_empty_user()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.IsType<MatchDto>(result);
        Assert.NotNull(result.User);
    }

    [Fact]
    public async Task given_match_exists_and_auth_service_fail_get_match_handler_should_return_Unauthorized_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_user_has_photos_get_match_handler_should_return_match_dto_with_non_empty_user_photos()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user2.Id);

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.Single(result.User.Photos);
        Assert.IsType<PhotoDto>(result.User.Photos.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_exists_and_match_has_messages_get_match_handler_should_return_match_dto_with_non_empty_messages()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id, "abc");

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.Single(result.Messages);
        Assert.IsType<MessageDto>(result.Messages.FirstOrDefault());
    }

    [Fact]
    public async Task get_match_handler_respects_how_many_messages_query_argument()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var match = await IntegrationTestHelper.CreateMatchAsync(_testDb, user1.Id, user2.Id);
        for (int i=0; i<20; i++)
        {
            _ = await IntegrationTestHelper.CreateMessageAsync(_testDb, match.Id, user2.Id, "abc");
        }

        int howManyMessages = 5;
        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id, HowManyMessages = howManyMessages };
        var result = await _handler.HandleAsync(query);
        Assert.NotNull(result);
        Assert.Equal(howManyMessages, result.Messages.Count());
        Assert.IsType<MessageDto>(result.Messages.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_not_exists_get_match_handler_should_return_match_not_exists_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var query = new GetMatch{ MatchId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));
        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly GetMatchHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetMatchHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetMatchHandler(_testDb.DbContext, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}