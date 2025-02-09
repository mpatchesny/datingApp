using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using datingApp.Infrastructure.Services;
using datingApp.Tests.Unit.Mocks;
using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class DeletePhotoHandlerTests
{
    [Fact]
    public async Task given_user_or_photo_not_exist_and_photo_id_not_in_deleted_entities_DeletePhotoHandler_returns_PhotoNotExistsException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(null));

        var fileStorageService = new Mock<IBlobStorage>();

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeletePhoto(Guid.NewGuid());
        var handler = new DeletePhotoHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_or_photo_not_exists_and_photo_id_in_deleted_entitites_DeletePhotoHandler_returns_PhotoAlreadyDeletedException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(null));

        var fileStorageService = new Mock<IBlobStorage>();

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>()));
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(true));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeletePhoto(Guid.NewGuid());
        var handler = new DeletePhotoHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoAlreadyDeletedException>(exception);
    }

    [Fact]
    public async Task given_authorization_failed_DeletePhotoHandler_returns_UnauthorizedException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(user));

        var fileStorageService = new Mock<IBlobStorage>();

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>()));
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new DeletePhoto(Guid.NewGuid());
        var handler = new DeletePhotoHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_photo_id_not_in_deleted_entities_and_authorization_succeed_DeletePhotoHandler_should_succeed()
    {
        var photo = new Photo(Guid.NewGuid(), "url", "checksum", 0);
        var photos = new List<Photo>(){ photo };
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, photos: photos);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(user));

        var fileStorageService = new MockFileStorageService();

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>()));

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeletePhoto(photo.Id);
        var handler = new DeletePhotoHandler(repository.Object, fileStorageService, deletedEntityService.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        repository.Verify(x => x.GetByPhotoIdAsync(photo.Id), Times.Once());
        repository.Verify(x => x.UpdateAsync(user), Times.Once());
        deletedEntityService.Verify(x => x.ExistsAsync(command.PhotoId), Times.Never());
        deletedEntityService.Verify(x => x.AddAsync(photo.Id), Times.Once());
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy"), Times.Once());
        Assert.Single(fileStorageService.DeletedItems);
        Assert.Null(user.Photos.FirstOrDefault(x => x.Id == photo.Id));
    }
}