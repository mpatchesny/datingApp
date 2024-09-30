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
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeleteUserHandlerTests : IDisposable
{
    [Fact]
    public async void given_user_exists_delete_user_should_succeed_and_add_user_id_to_deleted_entities()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        Assert.True(await _testDb.DbContext.DeletedEntities.AnyAsync(x => x.Id == user.Id));
        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async void given_authorization_fail_delete_existing_user_should_throw_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async void given_user_exists_delete_user_should_delete_user_photos_stored_on_disk()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photos = new List<Photo> { 
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id),
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id),
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id),
        };

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Exactly(3));
        foreach (var photo in photos)
        {
            _mockFileStorageService.Verify(x => x.DeleteFile(photo.Id.ToString()), Times.Once);
        }
    }

    [Fact]
    public async void given_user_not_exists_delete_user_throws_UserNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
    
        var command = new DeleteUser(Guid.NewGuid());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async void given_user_id_exists_in_deleted_entities_repository_delete_user_throws_UserAlreadyDeletedExceptionn()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        await IntegrationTestHelper.DeleteUserAsync(_testDb, user);

        var command = new DeleteUser(user.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserAlreadyDeletedException>(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DeleteUserHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    public DeleteUserHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();
        var userRepository = new DbUserRepository(_testDb.DbContext);
        var deletedEntitiesRepository = new DbDeletedEntityRepository(_testDb.DbContext);
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockFileStorageService.Setup(m => m.DeleteFile(It.IsAny<string>()));
        _handler = new DeleteUserHandler(userRepository, _mockFileStorageService.Object, deletedEntitiesRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}