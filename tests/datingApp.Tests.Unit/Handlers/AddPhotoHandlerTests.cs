using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using FluentStorage.Blobs;
using Moq;
using Npgsql.Replication.PgOutput.Messages;
using Xunit;

namespace datingApp.Tests.Unit.Handlers;

public class AddPhotoHandlerTests
{

    [Fact]
    public async void given_user_not_exists_AddPhotoHandler_returns_UserNotExistsException()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(null));

        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), new MemoryStream());
        var handler = new AddPhotoHandler(repository.Object, _photoValidator.Object, _fileStorage.Object,
            _jpegPhotoConverter.Object, _photoStorageUrlProvider.Object, _duplicateChecker.Object);

        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_exists_AddPhotoHandler_should_succeed()
    {
        var settings = new UserSettings(Guid.NewGuid(), PreferredSex.Female, new PreferredAge(18, 20), 20, new Location(45.5, 45.5));
        var user = new User(settings.UserId, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), UserSex.Male, settings);
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>())).Returns(Task.FromResult<User>(user));

        var convertedOutputSream = new MemoryStream();
        var jpegPhotoConverter = new Mock<IPhotoConverter>();
        jpegPhotoConverter.Setup(x => x.ConvertAsync(It.IsAny<Stream>())).Returns(Task.FromResult<Stream>(convertedOutputSream));

        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), new MemoryStream());
        var handler = new AddPhotoHandler(repository.Object, _photoValidator.Object, _fileStorage.Object,
            jpegPhotoConverter.Object, _photoStorageUrlProvider.Object, _duplicateChecker.Object);

        await handler.HandleAsync(command);

        var extension = "jpg";
        var expectedUrl = command.PhotoId.ToString() + extension;
        _photoValidator.Verify(x => x.Validate(command.PhotoStream, out extension), Times.Once());
        jpegPhotoConverter.Verify(x => x.ConvertAsync(command.PhotoStream), Times.Once());
        _photoStorageUrlProvider.Verify(x => x.GetPhotoUrl(command.PhotoId.ToString(), extension), Times.Once());
        Assert.Equal(command.PhotoId, user.Photos.FirstOrDefault().Id.Value);
        Assert.Equal(expectedUrl, user.Photos.FirstOrDefault().Url);
        Assert.Equal(0, user.Photos.FirstOrDefault().Oridinal.Value);
        _fileStorage.Verify(x => x.WriteAsync(It.IsAny<string>(), convertedOutputSream, false, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.UpdateAsync(user), Times.Once());
    }

    private readonly Mock<IPhotoValidator> _photoValidator;
    private Mock<IBlobStorage> _fileStorage;
    private Mock<IPhotoDuplicateChecker> _duplicateChecker;
    private readonly Mock<IPhotoConverter> _jpegPhotoConverter;
    private Mock<IPhotoUrlProvider> _photoStorageUrlProvider;
    public AddPhotoHandlerTests()
    {
        var extension = "jpg";
        _photoValidator = new Mock<IPhotoValidator>();
        _photoValidator.Setup(x => x.Validate(It.IsAny<Stream>(), out extension));

        _duplicateChecker = new Mock<IPhotoDuplicateChecker>();
        _duplicateChecker.Setup(x => x.IsDuplicate(It.IsAny<Guid>(), It.IsAny<Stream>())).Returns(Task.FromResult<Boolean>(false));

        _fileStorage = new Mock<IBlobStorage>();
        _fileStorage.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Stream>(), false, It.IsAny<CancellationToken>()));
        
        var convertedOutputSream = new MemoryStream();
        _jpegPhotoConverter = new Mock<IPhotoConverter>();
        _jpegPhotoConverter.Setup(x => x.ConvertAsync(It.IsAny<Stream>())).Returns(Task.FromResult<Stream>(convertedOutputSream));

        _photoStorageUrlProvider = new Mock<IPhotoUrlProvider>();
        _photoStorageUrlProvider.Setup(x => x.GetPhotoUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string firstArg, string secondArg) => firstArg + secondArg);
    }
}