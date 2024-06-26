using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace datingApp.Tests.Integration.CommandHandlers;


public class DeleteUserHandlerTests : IDisposable
{
    [Fact]
    public async void delete_existing_user_should_succeed()
    {
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new DeleteUser(Guid.Parse("00000000-0000-0000-0000-000000000001"))));
        Assert.Null(exception);

        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async void delete_existing_user_should_delete_user_photos_stored_on_disk()
    {
        var photos = new List<Photo> {
            new Photo(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1),
            new Photo(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1),
            new Photo(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1)
        };
        await _testDb.DbContext.Photos.AddRangeAsync(photos);
        await _testDb.DbContext.SaveChangesAsync();

        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new DeleteUser(Guid.Parse("00000000-0000-0000-0000-000000000001"))));
        Assert.Null(exception);

        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Exactly(3));
        foreach (var photo in photos)
        {
            _mockFileStorageService.Verify(x => x.DeleteFile(photo.Id.ToString()), Times.Once);
        }
    }

    [Fact]
    public async void delete_nonexisting_user_should_throw_exception()
    {
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(new DeleteUser(Guid.Parse("00000000-0000-0000-0000-000000000002"))));
        Assert.NotNull(exception);
        Assert.IsType<UserNotExistsException>(exception);
        _mockFileStorageService.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    // Arrange
    private readonly DeleteUserHandler _handler;
    private readonly TestDatabase _testDb;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    public DeleteUserHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var userRepository = new PostgresUserRepository(_testDb.DbContext);
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockFileStorageService.Setup(m => m.DeleteFile(It.IsAny<string>()));
        _handler = new DeleteUserHandler(userRepository, _mockFileStorageService.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}