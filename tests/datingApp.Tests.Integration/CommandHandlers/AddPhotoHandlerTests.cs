using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class AddPhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task given_user_exists_add_photo_to_user_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var command = new AddPhoto(Guid.NewGuid(), user.Id, "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_user_not_exists_add_photo_to_user_throws_UserNotExistsException()
    {
        var command = new AddPhoto(Guid.NewGuid(), Guid.NewGuid(), "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }
   
    [Fact]
    public async Task given_user_reached_photo_count_limit_add_photo_throws_UserPhotoLimitException()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        for (int i = 0; i < PHOTO_COUNT_PER_USER_LIMIT; i++)
        {
            _ = IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id, i);
        }
        
        var command = new AddPhoto(Guid.NewGuid(), user.Id, "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserPhotoLimitException>(exception);
    }
    
    // Arrange
    private readonly int PHOTO_COUNT_PER_USER_LIMIT = 6;
    private readonly AddPhotoHandler _handler;
    private readonly TestDatabase _testDb;
    public AddPhotoHandlerTests()
    {
        _testDb = new TestDatabase();
        var photoRepository = new DbPhotoRepository(_testDb.DbContext);
        var userRepository = new DbUserRepository(_testDb.DbContext);
        var mockedPhotoService = new Mock<IPhotoService>();
        mockedPhotoService.Setup(m => m.GetImageFileFormat()).Returns("jpg");
        var mockedFileStorage = new Mock<IFileRepository>();
        _handler = new AddPhotoHandler(photoRepository, userRepository, mockedPhotoService.Object, mockedFileStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}