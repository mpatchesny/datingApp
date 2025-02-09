using System;
using System.Collections.Generic;
using System.Linq;
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
using datingApp.Tests.Unit.Mocks;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class DeleteUserHandlerTests
{
    [Fact]
    public async Task given_user_not_exists_and_user_id_not_in_deleted_entities_DeleteUserHandler_returns_UserNotExistsException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var swipeRepository = new Mock<ISwipeRepository>();

        var fileStorageService = new Mock<IBlobStorage>();

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteUser(Guid.NewGuid());
        var handler = new DeleteUserHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object, swipeRepository.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_not_exists_and_user_id_in_deleted_entities_DeleteUserHandler_returns_UserAlreadyDeletedException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(true));

        var swipeRepository = new Mock<ISwipeRepository>();

        var fileStorageService = new Mock<IBlobStorage>();

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteUser(Guid.NewGuid());
        var handler = new DeleteUserHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object, swipeRepository.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserAlreadyDeletedException>(exception);
    }

    [Fact]
    public async Task given_authorization_fail_DeleteUserHandler_returns_UnauthorizedException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));

        var swipeRepository = new Mock<ISwipeRepository>();

        var fileStorageService = new Mock<IBlobStorage>();

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new DeleteUser(Guid.NewGuid());
        var handler = new DeleteUserHandler(repository.Object, fileStorageService.Object, deletedEntityService.Object, authorizationService.Object, swipeRepository.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_authorization_succeed_DeleteUserHandler_deletes_user()
    {
        var photos = new List<Photo>()
        {
            new Photo(Guid.NewGuid(), "url", "checksum", 0),
            new Photo(Guid.NewGuid(), "url", "checksum", 1),
        };
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, photos: photos);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        repository.Setup(x => x.DeleteAsync(It.IsAny<User>()));

        var deletedEntityService = new Mock<IDeletedEntityService>();
        deletedEntityService.Setup(x => x.ExistsAsync(It.IsAny<Guid>())).Returns(Task.FromResult<bool>(false));
        deletedEntityService.Setup(x => x.AddAsync(It.IsAny<Guid>()));

        var swipeRepository = new Mock<ISwipeRepository>();
        swipeRepository.Setup(x => x.DeleteUserSwipes(It.IsAny<UserId>()));

        var fileStorageService = new MockFileStorageService();

        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new DeleteUser(Guid.NewGuid());
        var handler = new DeleteUserHandler(repository.Object, fileStorageService, deletedEntityService.Object, authorizationService.Object, swipeRepository.Object);

        await handler.HandleAsync(command);
        repository.Verify(x => x.GetByIdAsync(command.UserId), Times.Once());
        repository.Verify(x => x.DeleteAsync(user), Times.Once());
        swipeRepository.Verify(x => x.DeleteUserSwipes(user.Id), Times.Once());
        deletedEntityService.Verify(x => x.ExistsAsync(command.UserId), Times.Never());
        deletedEntityService.Verify(x => x.AddAsync(user.Id), Times.Once());
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy"), Times.Once());
        Assert.Equal(2, fileStorageService.DeletedItems.Count);
    }
}