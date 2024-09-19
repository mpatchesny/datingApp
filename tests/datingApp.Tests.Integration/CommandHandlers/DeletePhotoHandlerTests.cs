using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeletePhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task delete_existing_photo_should_succeed_and_add_deleted_photo_id_to_deleted_entities()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var command = new DeletePhoto(photo.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        Assert.True(await _testDb.DbContext.DeletedEntities.AnyAsync(x => x.Id == photo.Id));
        _mockFileStorageService.Verify(x => x.DeleteFile(photo.Id.ToString()), Times.Once);
    }

    [Fact]
    public async Task given_authorization_fail_delete_existing_photo_should_throw_UnauthorizedException()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Failed()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var command = new DeletePhoto(photo.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task delete_nonexisting_photo_should_throw_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var nonExistingPhotoId = Guid.NewGuid();

        var command = new DeletePhoto(nonExistingPhotoId);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(nonExistingPhotoId.ToString()), Times.Never);
    }

    [Fact]
    public async Task given_photo_id_exists_in_deleted_entities_repository_delete_user_should_throw_already_deleted_exception()
    {
        _authService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Photo>(), "OwnerPolicy")).Returns(Task.FromResult(AuthorizationResult.Success()));
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var alreadyDeletedPhoto = new DeletedEntityDto() { Id = photo.Id };
        await _testDb.DbContext.DeletedEntities.AddAsync(alreadyDeletedPhoto);
        await _testDb.DbContext.SaveChangesAsync();

        var command = new DeletePhoto(alreadyDeletedPhoto.Id);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoAlreadyDeletedException>(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(alreadyDeletedPhoto.Id.ToString()), Times.Never);
    }
        
    // Arrange
    private readonly TestDatabase _testDb;
    private readonly DeletePhotoHandler _handler;
    private readonly Mock<IDatingAppAuthorizationService> _authService;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    public DeletePhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _authService = new Mock<IDatingAppAuthorizationService>();

        var photoRepository = new DbPhotoRepository(_testDb.DbContext);
        var deletedEntitiesRepository = new DbDeletedEntityRepository(_testDb.DbContext);
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockFileStorageService.Setup(m => m.DeleteFile(It.IsAny<string>()));
        _handler = new DeletePhotoHandler(photoRepository, _mockFileStorageService.Object, deletedEntitiesRepository, _authService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}