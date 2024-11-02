using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeletePhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task given_photo_exists_delete_photo_should_succeed_and_add_deleted_photo_id_to_deleted_entities_and_delete_photo_file_from_storage()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_dbContext, user.Id);
        _dbContext.ChangeTracker.Clear();

        var command = new DeletePhoto(photo.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        Assert.True(await _testDb.DbContext.DeletedEntities.AnyAsync(x => x.Id.Equals(photo.Id)));
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task given_authorization_fail_delete_existing_photo_throws_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_dbContext, user.Id);
        _dbContext.ChangeTracker.Clear();

        var command = new DeletePhoto(photo.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task given_photo_not_exists_delete_photo_throws_PhotoNotExistsException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var nonExistingPhotoId = Guid.NewGuid();

        var command = new DeletePhoto(nonExistingPhotoId);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task given_photo_not_exists_and_photo_id_in_deleted_entities_repository_delete_photo_throws_PhotoAlreadyDeletedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_dbContext, user.Id);
        await IntegrationTestHelper.DeletePhotoAsync(_dbContext, photo);
        _dbContext.ChangeTracker.Clear();

        var command = new DeletePhoto(photo.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoAlreadyDeletedException>(exception);
        _mockStorage.Verify(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly DeletePhotoHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    private readonly Mock<IBlobStorage> _mockStorage;
    public DeletePhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _authService = new Mock<IDatingAppAuthorizationService>();

        var userRepository = new DbUserRepository(_dbContext);
        var deletedEntitiesRepository = new DbDeletedEntityRepository(_dbContext);
        _mockStorage = new Mock<IBlobStorage>();
        _mockStorage.Setup(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()));
        _handler = new DeletePhotoHandler(userRepository, _mockStorage.Object, deletedEntitiesRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}