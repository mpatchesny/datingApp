using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class SwipeUserHandlerTests : IDisposable
{
    [Fact]
    public async void given_SwipedWhoId_user_not_exsits_SwipeUserHandler_not_throws_exception_and_returns_false_is_liked_by_other_user()
    {
        var swipeRepository = new DbSwipeRepository(_dbContext);
        var matchRepository = new DbMatchRepository(_dbContext);
        var isLikedByOtherUserStorage = new Mock<IIsLikedByOtherUserStorage>();
        var isLikedByOtherUser = (IsLikedByOtherUserDto) null;
        isLikedByOtherUserStorage.Setup(x => x.Set(It.IsAny<IsLikedByOtherUserDto>()))
            .Callback<IsLikedByOtherUserDto>(s => isLikedByOtherUser = s);
        var handler = new SwipeUserHandler(swipeRepository, matchRepository, isLikedByOtherUserStorage.Object);
        
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var nonExistingUserId = Guid.NewGuid();
        var swipe = await IntegrationTestHelper.CreateSwipeAsync(_dbContext, nonExistingUserId, user1.Id, Like.Like);

        var command = new SwipeUser(user1.Id, nonExistingUserId, (int) Like.Like);
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        Assert.Null(exception);
        Assert.False(isLikedByOtherUser.IsLikedByOtherUser);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    public SwipeUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;

    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}