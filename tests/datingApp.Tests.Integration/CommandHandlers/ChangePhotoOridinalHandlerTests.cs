using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Moq;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

[Collection("Integration tests")]
public class ChangePhotoOridinalHandlerTests : IDisposable
{
    [Fact]
    public async Task change_oridinal_of_existing_photo_should_succeed()
    {
        var command = new ChangePhotoOridinal(Guid.Parse("00000000-0000-0000-0000-000000000001"), 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_oridinal_of_existing_photo_when_other_photo_with_that_oridinal_exists_should_succeed()
    {
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();
        var command = new ChangePhotoOridinal(Guid.Parse("00000000-0000-0000-0000-000000000001"), 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_oridinal_of_nonexisting_photo_should_throw_exception()
    {
        var command = new ChangePhotoOridinal(Guid.Parse("00000000-0000-0000-0000-000000000002"), 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }
        
    // Arrange
    private readonly ChangePhotoOridinalHandler _handler;
    private readonly TestDatabase _testDb;
    public ChangePhotoOridinalHandlerTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 0);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();

        var mockedPhotoOrderer = new Mock<IPhotoOrderer>();
        mockedPhotoOrderer.Setup(m => m.OrderPhotos(It.IsAny<List<Photo>>(), It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns((List<Photo> x, Guid y, int z) => x);

        var photoRepository = new PostgresPhotoRepository(_testDb.DbContext);
        _handler = new ChangePhotoOridinalHandler(photoRepository, mockedPhotoOrderer.Object);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}