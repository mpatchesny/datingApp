using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class PhotoRepositoryTests : IDisposable
{
    [Fact]
    public async Task get_existing_photo_by_user_id_should_return_nonempty_collection()
    {
        var photos = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Single(photos);
    }

    [Fact]
    public async Task get_existing_photo_by_nonexisting_user_id_should_return_empty_collection()
    {
        var photos = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Empty(photos);
    }

    [Fact]
    public async Task add_photo_should_succeed()
    {
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(photo));
        Assert.Null(exception);
        var addedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Same(photo, addedPhoto);
    }

    [Fact]
    public async Task after_add_photo_get_photo_by_user_id_returns_plus_one_elements()
    {
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
        await _repository.AddAsync(photo);
        var photos = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Equal(2, photos.Count());
    }

    [Fact]
    public async Task after_delete_photo_get_photo_by_user_id_returns_minus_one_elements()
    {
        var photo = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        await _repository.DeleteAsync(photo);
        var photos = await _repository.GetByUserIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Empty(photos);
    }

    [Fact]
    public async Task add_photo_with_existing_id_should_throw_exception()
    {
        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(photo));
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task update_photo_should_succeed()
    {
        var photo = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        photo.ChangeOridinal(3);
        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(photo));
        Assert.Null(exception);
        var updatedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Same(photo, updatedPhoto);
    }

    [Fact]
    public async Task delete_existing_photo_should_succeed()
    {
        var photo = await _repository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(photo));
        Assert.Null(exception);
        var deletedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Null(deletedPhoto);
    }

    // Arrange
    private readonly IPhotoRepository _repository;
    private readonly TestDatabase _testDb;
    public PhotoRepositoryTests()
    {
        var settings = new UserSettings(Guid.Parse("00000000-0000-0000-0000-000000000001"), Sex.Female, 18, 20, 50, 40.5, 40.5);
        var user = new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "123456789", "test@test.com", "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        _testDb = new TestDatabase();
        _testDb.DbContext.Users.Add(user);
        _testDb.DbContext.SaveChanges();

        var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 1);
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