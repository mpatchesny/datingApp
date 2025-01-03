using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Storage;
using datingApp.Core.Consts;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class SwipeUserHandlerTests : IDisposable
{
    [Fact]
    public async void given_liked_user_deleted_his_account_Like_user_not_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var userWhoDeletedAccount = await IntegrationTestHelper.CreateUserAsync(_dbContext); 
        _ = IntegrationTestHelper.CreateSwipeAsync(_dbContext, userWhoDeletedAccount.Id, user.Id, Like.Like);
        await IntegrationTestHelper.DeleteUserAsync(_dbContext, userWhoDeletedAccount);
        _dbContext.ChangeTracker.Clear();

        var command = new SwipeUser(user.Id, userWhoDeletedAccount.Id, (int) Like.Like);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));

        Assert.Null(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly SwipeUserHandler _handler;
    public SwipeUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var swipeRepository = new DbSwipeRepository(_dbContext);
        var matchRepository = new DbMatchRepository(_dbContext);
        var deletedEntityService = new DeletedEntityService(_dbContext);
        var isLikedByOtherUserStorage = new Moq.Mock<IIsLikedByOtherUserStorage>();
        _handler = new SwipeUserHandler(swipeRepository, matchRepository, deletedEntityService, isLikedByOtherUserStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}