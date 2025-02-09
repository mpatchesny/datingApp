using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class ChangePhotoOridinalHandlerTests
{
    [Fact]
    public async Task given_user_not_exists_ChangePhotoOridinalHandler_returns_PhotoNotExistsException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(null));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangePhotoOridinal(Guid.NewGuid(), 1);
        var handler = new ChangePhotoOridinalHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    [Fact]
    public async Task given_photo_not_exists_ChangePhotoOridinalHandler_returns_PhotoNotExistsException()
    {
        var existingPhoto = new Photo(Guid.NewGuid(), "url", "checksum", 0);
        var notExistingPhotoId = Guid.NewGuid();
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings,
            photos: new List<Photo>(){ existingPhoto });
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(existingPhoto.Id)).Returns(Task.FromResult<User>(user));
        repository.Setup(x => x.GetByPhotoIdAsync(notExistingPhotoId)).Returns(Task.FromResult<User>(null));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangePhotoOridinal(notExistingPhotoId, 1);
        var handler = new ChangePhotoOridinalHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }

    [Fact]
    public async Task given_authorization_failed_ChangePhotoOridinalHandler_returns_UnauthorizedException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(It.IsAny<PhotoId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new ChangePhotoOridinal(Guid.NewGuid(), 1);
        var handler = new ChangePhotoOridinalHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_authorization_succeed_ChangePhotoOridinalHandler_should_succeed()
    {
        var photos = new List<Photo>()
        {
            new Photo(Guid.NewGuid(), "url", "checksum", 0),
            new Photo(Guid.NewGuid(), "url", "checksum", 1),
        };
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings,
            photos: photos);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByPhotoIdAsync(photos[0].Id)).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangePhotoOridinal(photos[0].Id, 1);
        var handler = new ChangePhotoOridinalHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        repository.Verify(x => x.GetByPhotoIdAsync(photos[0].Id), Times.Once());
        repository.Verify(x => x.UpdateAsync(user), Times.Once());
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy"), Times.Once());
        Assert.Equal(1, user.Photos.FirstOrDefault(x => x.Id == photos[0].Id).Oridinal.Value);
    }
}