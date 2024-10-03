using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Consts;
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
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var photos = await _repository.GetByUserIdAsync(user.Id);
        Assert.Single(photos);
    }

    [Fact]
    public async Task get_existing_photo_by_nonexisting_user_id_should_return_empty_collection()
    {
        var photos = await _repository.GetByUserIdAsync(Guid.NewGuid());
        Assert.Empty(photos);
    }

    [Fact]
    public async Task add_photo_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        var photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        var photo = new Photo(Guid.NewGuid(), user.Id, "abc", 1, photoFile);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(photo));
        Assert.Null(exception);
        var addedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == photo.Id);
        Assert.Same(photo, addedPhoto);
    }

    [Fact]
    public async Task after_add_photo_get_photo_by_user_id_returns_plus_one_elements()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        _ = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        var photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        var photo = new Photo(Guid.NewGuid(), user.Id, "abc", 1, photoFile);
        await _repository.AddAsync(photo);
        var photos = await _repository.GetByUserIdAsync(user.Id);
        Assert.Equal(2, photos.Count());
    }

    [Fact]
    public async Task after_delete_photo_get_photo_by_user_id_returns_minus_one_elements()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        await _repository.DeleteAsync(photo);
        var photos = await _repository.GetByUserIdAsync(user.Id);
        Assert.Empty(photos);
    }

    [Fact]
    public async Task add_photo_with_existing_id_throws_exception()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        byte[] bytes = new byte[10241];
        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        var photoFile = new PhotoFile(Guid.NewGuid(), bytes);
        var badPhoto = new Photo(photo.Id, user.Id, "abc", 1, photoFile);
        var exception = await Record.ExceptionAsync(async () => await _repository.AddAsync(photo));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task update_photo_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        photo.ChangeOridinal(3);
        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateAsync(photo));
        Assert.Null(exception);
        var updatedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == photo.Id);
        Assert.Same(photo, updatedPhoto);
    }

    [Fact]
    public async Task update_photos_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photos = new List<Photo>{
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id),
            await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id),
        };

        photos[0].ChangeOridinal(2);
        photos[1].ChangeOridinal(1);
        var exception = await Record.ExceptionAsync(async () => await _repository.UpdateRangeAsync(photos.ToArray()));
        Assert.Null(exception);
        var updatedPhotos = await _testDb.DbContext.Photos.Where(x => x.UserId == user.Id).ToListAsync();
        Assert.Equal(photos.OrderBy(x => x.Id), updatedPhotos.OrderBy(x => x.Id));
    }

    [Fact]
    public async Task delete_existing_photo_should_succeed()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(photo));
        Assert.Null(exception);
        var deletedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == photo.Id);
        Assert.Null(deletedPhoto);
    }

    [Fact]
    public async Task delete_existing_photo_should_delete_associated_file()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var exception = await Record.ExceptionAsync(async () => await _repository.DeleteAsync(photo));
        Assert.Null(exception);
        var deletedPhoto = await _testDb.DbContext.Photos.FirstOrDefaultAsync(x => x.Id == photo.Id);
        Assert.Null(deletedPhoto);
        var deletedFile = await _testDb.DbContext.PhotoFiles.FirstOrDefaultAsync(f => f.PhotoId == photo.Id);
        Assert.Null(deletedFile);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    private readonly IPhotoRepository _repository;
    public PhotoRepositoryTests()
    {
        _testDb = new TestDatabase();
        _repository = new DbPhotoRepository(_testDb.DbContext);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}