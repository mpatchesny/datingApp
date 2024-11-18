using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeleteUserHandlerTests : IDisposable
{
    [Fact]
    public async void given_user_exists_delete_user_should_succeed_and_add_user_id_to_deleted_entities_and_delete_user_photo_files_from_storage()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var photos = new List<Photo> { 
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto(),
            IntegrationTestHelper.CreatePhoto(),
        };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        Assert.True(await _testDb.DbContext.DeletedEntities.AnyAsync(x => x.Id == user.Id.Value));
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void given_authorization_fail_delete_existing_user_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async void given_user_not_exists_delete_user_throws_UserNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
    
        var command = new DeleteUser(Guid.NewGuid());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async void given_user_not_exists_and_id_in_deleted_entities_repository_delete_user_throws_UserAlreadyDeletedExceptionn()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        await IntegrationTestHelper.DeleteUserAsync(_dbContext, user);
        _dbContext.ChangeTracker.Clear();

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserAlreadyDeletedException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly DeleteUserHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    private readonly Mock<IBlobStorage> _mockStorage;
    public DeleteUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _authService = new Mock<IDatingAppAuthorizationService>();
        var userRepository = new DbUserRepository(_dbContext);
        var deletedEntitiesRepository = new DeletedEntityService(_dbContext);
        _mockStorage = new Mock<IBlobStorage>();
        _mockStorage.Setup(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()));
        _handler = new DeleteUserHandler(userRepository, _mockStorage.Object, deletedEntitiesRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}