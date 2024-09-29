using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class SwipeUserHandlerTests : IDisposable
{
    [Theory]
    [InlineData(Like.Like)]
    [InlineData(Like.Pass)]
    public async Task given_users_exist_add_swipe_should_succeed(Like like)
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new SwipeUser(user1.Id, user2.Id, (int) like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(Like.Like)]
    [InlineData(Like.Pass)]
    public async Task add_swipe_should_add_IsLikedByOtherUserDto_to_storage_only_once_with_propero_IsLikedByOtherUserValue(Like like)
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new SwipeUser(user1.Id, user2.Id, (int) like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _isLikedByOtherUserStorage.Verify(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()), Times.Once);
    }

    [Fact]
    public async Task given_swipe_with_like_value_from_other_user_exists_add_swipe_with_like_value_should_create_new_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_testDb, user2.Id, user1.Id, Like.Like);

        var command = new SwipeUser(user1.Id, user2.Id, 1);
        await _handler.HandleAsync(command);
        var match = await _testDb.DbContext.Matches.FirstOrDefaultAsync(m => (m.UserId1 == user1.Id && m.UserId2 == user2.Id) || 
            (m.UserId1 == user2.Id && m.UserId2 == user1.Id) );
        Assert.NotNull(match);
        Assert.Equal(match.UserId1, user1.Id);
        Assert.Equal(match.UserId2, user1.Id);
    }

    [Fact]
    public async Task given_user_does_not_exists_add_swipe_should_not_throw_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new SwipeUser(user1.Id, Guid.NewGuid(), 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly SwipeUserHandler _handler;
    private readonly Mock<IIsLikedByOtherUserStorage> _isLikedByOtherUserStorage;
    public SwipeUserHandlerTests()
    {
        _testDb = new TestDatabase();
        var swipeRepository = new DbSwipeRepository(_testDb.DbContext);
        var matchRepository = new DbMatchRepository(_testDb.DbContext);
        _isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        _handler = new SwipeUserHandler(swipeRepository, matchRepository, _isLikedByOtherUserStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}