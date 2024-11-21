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
    public async Task given_user_exists_add_photo_to_user_should_succeed_and_add_photo_file_to_storage()
    {
        // var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        // _dbContext.ChangeTracker.Clear();

        // var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SampleFileBase64Content());
        // var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        // Assert.Null(exception);

        // _mockStorage.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()), Times.Once());
        // _mockPhotoService.Verify(x => x.ProcessBase64Photo(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task given_user_not_exists_add_photo_to_user_throws_UserNotExistsException()
    {
        // var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), IntegrationTestHelper.SampleFileBase64Content());
        // var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        // Assert.NotNull(exception);
        // Assert.IsType<UserNotExistsException>(exception);
    }

    [Fact]
    public async Task given_user_exists_and_photo_service_process_photo_failed_add_photo_to_user_throws_exception()
    {
        // var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        // _dbContext.ChangeTracker.Clear();

        // var command = new AddPhoto(Guid.NewGuid(), user.Id, IntegrationTestHelper.SampleFileBase64Content());
        // _mockPhotoService.Setup(x => x.ProcessBase64Photo(It.IsAny<string>())).Throws(new InvalidPhotoException());

        // var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoException>(exception);
    }

    [Fact]
    public async Task given_user_not_exists_and_photo_service_process_photo_failed_add_photo_to_user_throws_exception()
    {
        // var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), IntegrationTestHelper.SampleFileBase64Content());
        // _mockPhotoService.Setup(x => x.ProcessBase64Photo(It.IsAny<string>())).Throws(new InvalidPhotoException());

        // var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        // Assert.NotNull(exception);
        // Assert.IsType<InvalidPhotoException>(exception);
    }

    // Arrange
    private readonly AddPhotoHandler _handler;
    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly Mock<IBlobStorage> _mockStorage;
    public AddPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        var userRepository = new DbUserRepository(_dbContext);
        _mockStorage = new Mock<IBlobStorage>();
        _mockStorage.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>(), false, It.IsAny<System.Threading.CancellationToken>()));
        // _handler = new AddPhotoHandler(userRepository, _mockPhotoService.Object, _mockStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}