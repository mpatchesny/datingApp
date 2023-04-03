using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;

[Collection("Integration tests")]
public class PhotoRepositoryTests : IDisposable
{
    [Fact]
    public async Task get_existing_photo_by_user_id_should_return_nonempty_collection()
    {
        var photos = await _repository.GetByUserIdAsync(1);
        Assert.Equal(1, photos.Count());
    }

    [Fact]
    public async Task get_existing_photo_by_nonexisting_user_id_should_return_empty_collection()
    {
        var photos = await _repository.GetByUserIdAsync(2);
        Assert.Equal(0, photos.Count());
    }

    [Fact]
    public async Task add_photo_should_succeed()
    {
        var photo = new Photo(0, 1, "abc", 1);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(photo));
        Assert.Null(exception);
    }

    [Fact]
    public async Task delete_existing_photo_should_succeed()
    {
        var photo = await _repository.GetByIdAsync(1);
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(photo));
        Assert.Null(exception);
    }

    // Arrange
    private readonly IPhotoRepository _repository;
    private readonly TestDatabase _testDb;
    public PhotoRepositoryTests()
    {
        var settings = new UserSettings(0, Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(0, "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var photo = new Photo(0, 1, "abc", 1);
        _testDb.DbContext.Photos.Add(photo);
        _testDb.DbContext.SaveChanges();
        _repository = new PostgresPhotoRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}