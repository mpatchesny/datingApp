using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class AddPhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task add_photo_to_existing_user_should_succeed()
    {
        var command = new AddPhoto(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task add_photo_to_nonexisting_user_should_throw_exception()
    {
        var command = new AddPhoto(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
    }
   
    [Fact]
    public async Task add_photo_when_user_reached_photo_count_limit_should_throw_exception()
    {
        var photos = new List<Photo>
        {
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 2),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 3),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 4),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 5),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 6)
        };
        await _testDb.DbContext.Photos.AddRangeAsync(photos);
        await _testDb.DbContext.SaveChangesAsync();
        var command = new AddPhoto(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "dGVzdA==");
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<UserPhotoLimitException>(exception);
    }
    
    // Arrange
    private readonly AddPhotoHandler _handler;
    private readonly TestDatabase _testDb;
    public AddPhotoHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000000"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();

        var photoRepository = new PostgresPhotoRepository(_testDb.DbContext);
        var userRepository = new PostgresUserRepository(_testDb.DbContext);

        var mockedPhotoService = new Mock<IPhotoService>();
        mockedPhotoService.Setup(m => m.GetImageFileFormat(It.IsAny<byte[]>())).Returns("jpg");
        var mockedFileStorage = new Mock<IFileStorage>();
        _handler = new AddPhotoHandler(photoRepository, userRepository, mockedPhotoService.Object, mockedFileStorage.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}