using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.CommandHandlers;

public class ChangePhotoOridinalHandlerTests
{
    [Fact]
    public async Task change_oridinal_of_existing_photo_should_succeed()
    {
        var command = new ChangePhotoOridinal(1, 1, 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_oridinal_of_existing_photo_when_other_photo_with_that_oridinal_exists_should_succeedAsync()
    {
        var photo = new Photo(0, 1, "abc", 2);
        var command = new ChangePhotoOridinal(1, 1, 2);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.Null(exception);
    }

    [Fact]
    public async Task change_oridinal_of_nonexisting_photo_should_throw_exceptionAsync()
    {
        var command = new ChangePhotoOridinal(1, 2, 1);
        var exception = await Record.ExceptionAsync(async () => await _handler.HandleAsync(command));
        Assert.NotNull(exception);
        Assert.IsType<PhotoNotExistsException>(exception);
    }
        
    // Arrange
    private readonly ChangePhotoOridinalHandler _handler;
    private readonly TestDatabase _testDb;
    public ChangePhotoOridinalHandlerTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);

        var photo = new Photo(0, 1, "abc", 1);

        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();

        var photoRepository = new PostgresPhotoRepository(_testDb.DbContext);
        _handler = new ChangePhotoOridinalHandler(photoRepository);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}