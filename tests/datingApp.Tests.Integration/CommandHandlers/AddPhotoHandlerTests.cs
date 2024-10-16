using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
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
    public async Task given_user_exists_add_photo_to_user_should_succeed_and_add_photo_file_to_storage()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SampleFileBase64Content());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);

        _mockStorage.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()), Times.Once());
        _mockPhotoService.Verify(x => x.ProcessBase64Photo(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task given_user_not_exists_add_photo_to_user_throws_UserNotExistsException()
    {
        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), IntegrationTestHelper.SampleFileBase64Content());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_photo_service_process_photo_failed_add_photo_to_user_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SampleFileBase64Content());
        _mockPhotoService.Setup(x => x.ProcessBase64Photo(It.IsAny<string>())).Throws(new InvalidPhotoException());

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public async Task given_user_not_exists_and_photo_service_process_photo_failed_add_photo_to_user_throws_exception()
    {
        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), IntegrationTestHelper.SampleFileBase64Content());
        _mockPhotoService.Setup(x => x.ProcessBase64Photo(It.IsAny<string>())).Throws(new InvalidPhotoException());

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<InvalidPhotoException>(exception);
    }
   
    [Fact]
    public async Task given_user_reached_photo_count_limit_add_photo_throws_UserPhotoLimitException()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        for (int i = 0; i < PHOTO_COUNT_PER_USER_LIMIT; i++)
        {
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, i);
        }
        
        var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SampleFileBase64Content());
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserPhotoLimitException>(exception);
    }
    
    // Arrange
    private readonly int PHOTO_COUNT_PER_USER_LIMIT = 6;
    private readonly AddPhotoHandler _handler;
    private readonly TestDatabase _testDb;
    private readonly Mock<IPhotoService> _mockPhotoService;
    private readonly Mock<IBlobStorage> _mockStorage;
    public AddPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        var photoRepository = new DbPhotoRepository(_testDb.DbContext);
        var userRepository = new DbUserRepository(_testDb.DbContext);
        _mockPhotoService = new Mock<IPhotoService>();
        _mockPhotoService.Setup(x => x.ProcessBase64Photo(It.IsAny<string>())).Returns(new PhotoServiceProcessOutput(new byte[10], "jpg"));
        _mockStorage = new Mock<IBlobStorage>();
        _mockStorage.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()));
        _handler = new AddPhotoHandler(photoRepository, userRepository, _mockPhotoService.Object, _mockStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}