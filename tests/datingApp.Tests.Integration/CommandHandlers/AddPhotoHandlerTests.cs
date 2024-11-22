using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using FluentStorage.Blobs;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class AddPhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_add_photo_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var extension = "jpg";
        _mockPhotoValidator.Setup(x => x.Validate(It.IsAny<Stream>(), out extension));

        var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SamplePhotoStream());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);

        var addedPhoto = _dbContext.Photos.FirstOrDefault(photo => photo.Id.Equals(command.PhotoId));
        Assert.Equal("foo.jpg", addedPhoto.Url);
        _mockStorage.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()), Times.Once());
        _mockPhotoValidator.Verify(x => x.Validate(It.IsAny<Stream>(), out extension), Times.Once());
        _mockPhotoConverter.Verify(x => x.ConvertAsync(It.IsAny<Stream>()), Times.Once());
        _mockUrlProdier.Verify(x => x.GetPhotoUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task given_user_not_exists_add_photo_to_user_throws_UserNotExistsException()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var extension = "jpg";
        _mockPhotoValidator.Setup(x => x.Validate(It.IsAny<Stream>(), out extension));

        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), IntegrationTestHelper.SamplePhotoStream());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_validate_photo_failed_add_photo_to_user_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var extension = "jpg";
        _mockPhotoValidator.Setup(x => x.Validate(It.IsAny<Stream>(), out extension)).Throws<InvalidPhotoException>();

        var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SamplePhotoStream());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    // Arrange
    private readonly AddPhotoHandler _handler;
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<IBlobStorage> _mockStorage;
    private readonly Mock<IPhotoValidator<Stream>> _mockPhotoValidator;
    private readonly Mock<IPhotoConverter> _mockPhotoConverter;
    private readonly Mock<IPhotoUrlProvider> _mockUrlProdier;

    public AddPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var userRepository = new DbUserRepository(_dbContext);

        _mockPhotoValidator = new Mock<IPhotoValidator<Stream>>();
        _mockStorage = new Mock<IBlobStorage>();
        _mockStorage.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()));
        _mockPhotoConverter = new Mock<IPhotoConverter>();

        _mockPhotoConverter.Setup(x => x.ConvertAsync(It.IsAny<Stream>())).Returns(Task.FromResult<Stream>(new MemoryStream()));
        _mockUrlProdier = new Mock<IPhotoUrlProvider>();
        _mockUrlProdier.Setup(x => x.GetPhotoUrl(It.IsAny<string>(), It.IsAny<string>())).Returns("foo.jpg");

        _handler = new AddPhotoHandler(userRepository, _mockPhotoValidator.Object, _mockStorage.Object, _mockPhotoConverter.Object, _mockUrlProdier.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}