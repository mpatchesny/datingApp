using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeletePhotoHandlerTests : IDisposable
{
    [Fact]
    public async Task delete_existing_photo_should_succeed()
    {
        var guidString = "00000000-0000-0000-0000-000000000001";
        var command = new DeletePhoto(Guid.Parse(guidString));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(guidString), Times.Once);
    }

    [Fact]
    public async Task delete_nonexisting_photo_should_throw_exception()
    {
        var guidString = "00000000-0000-0000-0000-000000000002";
        var command = new DeletePhoto(Guid.Parse(guidString));
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(guidString), Times.Never);
    }
        
    // Arrange
    private readonly DeletePhotoHandler _handler;
    private readonly TestDatabase _testDb;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    public DeletePhotoHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();

        var photoRepository = new PostgresPhotoRepository(_testDb.DbContext);
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockFileStorageService.Setup(m => m.DeleteFile(It.IsAny<string>()));
        _handler = new DeletePhotoHandler(photoRepository, _mockFileStorageService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}