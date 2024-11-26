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
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace datingApp.Tests.Integration.CommandHandlers;

public class SwipeUserHandlerTests : IDisposable
{
    [Theory]
    [InlineData(Like.Like)]
    [InlineData(Like.Pass)]
    public async Task given_users_exist_add_swipe_should_succeed(Like like)
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(Like.Like)]
    [InlineData(Like.Pass)]
    public async Task add_swipe_should_add_IsLikedByOtherUserDto_to_storage_only_once_with_false_IsLikedByOtherUserValue_if_other_users_not_liked(Like like)
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _isLikedByOtherUserStorage.Verify(x => x.Set(It.Is<IsLikedByOtherUserDto>(i => i.IsLikedByOtherUser == false)), Times.Once);
    }

    [Theory]
    [InlineData(Like.Like)]
    [InlineData(Like.Pass)]
    public async Task add_swipe_should_add_IsLikedByOtherUserDto_to_storage_only_once_with_true_IsLikedByOtherUserValue_if_other_users_liked(Like like)
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _isLikedByOtherUserStorage.Verify(x =>  x.Set(It.Is<IsLikedByOtherUserDto>(i => i.IsLikedByOtherUser == true)), Times.Once);
    }

    [Fact]
    public async Task given_swipe_with_like_value_from_other_user_exists_add_swipe_with_like_value_should_create_new_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Like);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) Like.Like);
        await _handler.HandleAsync(command);
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => (m.UserId1.Equals(user1.Id) && m.UserId2.Equals(user2.Id)) || 
            (m.UserId1.Equals(user2.Id) && m.UserId2.Equals(user2.Id)));
        Assert.NotNull(match);
        Assert.True(match.UserId1.Equals(user1.Id));
        Assert.True(match.UserId2.Equals(user2.Id));
    }

    [Fact]
    public async Task given_swipe_with_pass_value_from_other_user_exists_add_swipe_with_like_value_should_not_create_new_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) Like.Like);
        await _handler.HandleAsync(command);
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => (m.UserId1.Equals(user1.Id) && m.UserId2.Equals(user2.Id)) || 
            (m.UserId1.Equals(user2.Id) && m.UserId2.Equals(user1.Id)));
        Assert.Null(match);
    }

    [Fact]
    public async Task given_swipe_from_other_user_not_exists_add_swipe_with_like_value_should_not_create_new_match()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _ = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, user2.Id, user1.Id, Like.Pass);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, user2.Id, (int) Like.Like);
        await _handler.HandleAsync(command);
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => (m.UserId1.Equals(user1.Id) && m.UserId2.Equals(user2.Id)) || 
            (m.UserId1.Equals(user2.Id) && m.UserId2.Equals(user1.Id)));
        Assert.Null(match);
    }

    [Fact]
    public async Task given_user_does_not_exists_add_swipe_should_not_throw_exception()
    {
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user1.Id, Guid.NewGuid(), 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly SwipeUserHandler _handler;
    private readonly Mock<IIsLikedByOtherUserStorage> _isLikedByOtherUserStorage;

    public SwipeUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var swipeRepository = new DbSwipeRepository(_dbContext);
        var matchRepository = new DbMatchRepository(_dbContext);
        _isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        _handler = new SwipeUserHandler(swipeRepository, matchRepository, _isLikedByOtherUserStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}