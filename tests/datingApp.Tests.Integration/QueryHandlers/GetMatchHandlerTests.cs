using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Spatial;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Handlers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using Match = datingApp.Core.Entities.Match;

namespace datingApp.Tests.Integration.QueryHandlers;


public class GetMatchHandlerTests : IDisposable
{
    [Fact]
    public async Task given_match_exists_GetMatchHandler_should_return_match_dto_with_non_empty_user()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, isDisplayedByUser1: true);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.IsType<MatchDto>(result);
        Assert.NotNull(result.User);
        Assert.Equal(user2.Id.Value, result.User.Id);
        Assert.True(result.IsDisplayed);
    }

    [Fact]
    public async Task given_match_exists_and_authorization_failed_GetMatchHandler_should_return_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_match_exists_and_user_has_photos_GetMatchHandler_should_return_match_dto_with_non_empty_user_photos()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.Single(result.User.Photos);
        Assert.IsType<PhotoDto>(result.User.Photos.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_exists_and_match_has_messages_GetMatchHandler_should_return_match_dto_with_non_empty_messages_ordered_by_created_date_desc()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var createdTime = DateTime.UtcNow;
        var messages = new List<Message>() { 
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: createdTime.AddSeconds(-5)),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: createdTime.AddSeconds(-4)),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: createdTime.AddSeconds(-3)),
            IntegrationTestHelper.CreateMessage(user1.Id, createdAt: createdTime.AddSeconds(-2)),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: createdTime.AddSeconds(-1)),
            IntegrationTestHelper.CreateMessage(user2.Id, createdAt: createdTime.AddSeconds(0)),
        };
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Messages);
        Assert.IsType<MessageDto>(result.Messages.FirstOrDefault());
        Assert.Collection(result.Messages,
            m => Assert.Equal(messages[^1].Id.Value, m.Id),
            m => Assert.Equal(messages[^2].Id.Value, m.Id),
            m => Assert.Equal(messages[^3].Id.Value, m.Id),
            m => Assert.Equal(messages[^4].Id.Value, m.Id),
            m => Assert.Equal(messages[^5].Id.Value, m.Id),
            m => Assert.Equal(messages[^6].Id.Value, m.Id));
    }

    [Fact]
    public async Task GetMatchHandler_respects_how_many_messages_query_property()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var messages = new List<Message>();
        for (int i=0; i<20; i++)
        {
            messages.Add(IntegrationTestHelper.CreateMessage(user2.Id));
        }
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id, messages: messages);
        _dbContext.ChangeTracker.Clear();

        int howManyMessages = 5;
        var query = new GetMatch{ MatchId = match.Id, UserId = user1.Id, HowManyMessages = howManyMessages };
        var result = await _handler.HandleAsync(query);

        Assert.NotNull(result);
        Assert.Equal(howManyMessages, result.Messages.Count());
        Assert.IsType<MessageDto>(result.Messages.FirstOrDefault());
    }

    [Fact]
    public async Task given_match_not_exists_GetMatchHandler_returns_MatchNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = Guid.NewGuid(), UserId = user1.Id };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<MatchNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_not_exists_GetMatchHandler_returns_UserNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Match>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var match = await IntegrationTestHelper.CreateMatchAsync(_dbContext, user1.Id, user2.Id);
        _dbContext.ChangeTracker.Clear();

        var query = new GetMatch{ MatchId = match.Id, UserId = Guid.NewGuid() };
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(query));

        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<ISpatial> _mockedSpatial;
    private readonly GetMatchHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    public GetMatchHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _mockedSpatial = new Mock<ISpatial>();
        _mockedSpatial.Setup(m => m.CalculateDistanceInKms(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0);
        _authService = new Mock<IDatingAppAuthorizationService>();
        _handler = new GetMatchHandler(_testDb.DbContext, _authService.Object, _mockedSpatial.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}