using System;
using System.Collections.Generic;
using System.Linq;
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

public class ChangeUserHandlerTests
{
    [Fact]
    public async Task given_user_not_exists_ChangeUserHandler_returns_UserNotExistsException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(Guid.NewGuid());
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_authorization_fails_ChangeUserHandler_returns_UnauthorizedException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var command = new ChangeUser(Guid.NewGuid());
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UnauthorizedException>(exception);
    }

    [Fact]
    public async Task given_date_of_birth_not_in_ISO8601_format_ChangeUserHandler_throws_InvalidDateOfBirthException()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, "01-01-2000");
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidDateOfBirthFormatException>(exception);
    }

    [Fact]
    public async Task given_no_changes_ChangeUserHandler_should_succeed()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_Lat_and_Lon_not_changed_ChangeUserHandler_changes_nothing()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, Lat: 40.0);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        Assert.Equal(45.5, user.Settings.Location.Lat);
    }

    [Fact]
    public async Task given_Lon_and_Lat_not_changed_ChangeUserHandler_changes_nothing()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, Lon: 40.0);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        Assert.Equal(45.5, user.Settings.Location.Lon);
    }

    [Fact]
    public async Task given_PreferredAgeFrom_changed_and_PreferredAgeTo_not_ChangeUserHandler_changes_nothing()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, PreferredAgeFrom: 20);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        Assert.Equal(18, user.Settings.PreferredAge.From);
    }

    [Fact]
    public async Task given_PreferredAgeTo_changed_and_PreferredAgeFrom_not_ChangeUserHandler_changes_nothing()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, PreferredAgeTo: 40);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        Assert.Equal(20, user.Settings.PreferredAge.To);
    }

    [Fact]
    public async Task given_user_exists_and_authorization_succeed_ChangeUserHandler_should_change_User()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings, DateTime.UtcNow, job: "some job", bio: "some bio");
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));
        var authorizationService = new Mock<IDatingAppAuthorizationService>();
        authorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<User>(), "OwnerPolicy"))
            .Returns(Task.FromResult(AuthorizationResult.Success()));

        var command = new ChangeUser(user.Id, "2000-01-01", "new bio", "new job", 30, 40, 99, 1, 40.0, 41.1);
        var handler = new ChangeUserHandler(repository.Object, authorizationService.Object);

        await handler.HandleAsync(command);
        authorizationService.Verify(x => x.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy"), Times.Once());
        repository.Verify(x => x.GetByIdAsync(command.UserId), Times.Once());
        repository.Verify(x => x.UpdateAsync(user), Times.Once());
        Assert.Equal(command.Bio, user.Bio);
        Assert.Equal(command.Job, user.Job);
        Assert.Equal(command.DateOfBirth, user.DateOfBirth.Value.ToString("yyyy-MM-dd"));
        Assert.Equal(command.Lat, user.Settings.Location.Lat);
        Assert.Equal(command.Lon, user.Settings.Location.Lon);
        Assert.Equal(command.PreferredAgeFrom, user.Settings.PreferredAge.From);
        Assert.Equal(command.PreferredAgeTo, user.Settings.PreferredAge.To);
        Assert.Equal(command.PreferredMaxDistance, user.Settings.PreferredMaxDistance.Value);
        Assert.Equal(command.PreferredSex, (int) user.Settings.PreferredSex);
    }
}