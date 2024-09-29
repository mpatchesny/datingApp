using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Npgsql.Replication;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class FileRepositoryTests : IDisposable
{
    [Fact]
    public async Task given_file_exists_GetFileAsync_returns_file_binaryAsync()
    {
        var photo = await CreatePhotoAsync();
        var file = await CreateFileAsync(photo.Id);

        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        var fileBinary = await storage.GetByIdAsync(file.Id);
        Assert.Same(fileBinary, file.Binary);
    }

    [Fact]
    public async Task given_file_not_exists_GetFileAsync_returns_null()
    {
        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        var file = await storage.GetByIdAsync(Guid.NewGuid());
        Assert.Null(file);
    }

    [Fact]
    public async Task given_file_not_exists_SaveFileAsync_saves_file()
    {
        var photo = await CreatePhotoAsync();
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        var fileId = photo.Id;

        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.AddAsync(data, fileId , "txt"));
        Assert.Null(exception);

        var file = await storage.GetByIdAsync(fileId);
        Assert.NotNull(file);
    }

    [Fact]
    public async Task given_file_exists_SaveFileAsync_updates_file()
    {
        var photo = await CreatePhotoAsync();
        var file = await CreateFileAsync(photo.Id);

        file.Binary[0] = byte.MaxValue;
        file.Binary[1] = byte.MaxValue;
        file.Binary[2] = byte.MaxValue;

        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.AddAsync(file.Binary, file.Id , "txt"));
        Assert.Null(exception);

        var fileBinary = await storage.GetByIdAsync(file.Id);
        Assert.Same(file.Binary, fileBinary);
    }

    [Fact]
    public async Task given_file_exists_DeleteFileAsync_deletes_files()
    {
        var photo = await CreatePhotoAsync();
        var file = await CreateFileAsync(photo.Id);
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };

        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        await storage.DeleteAsync(file.Id);

        file = _testDb.DbContext.Files.FirstOrDefault(x => x.Id == file.Id);
        Assert.Null(file);
    }

    [Fact]
    public async Task given_file_notexists_DeleteFileAsync_not_throws_exepction()
    {
        IFileRepository storage = new DbFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.DeleteAsync(Guid.NewGuid()));
        Assert.Null(exception);
    }

    // Arrange
    private async Task<Photo> CreatePhotoAsync()
    {
        var userId = Guid.NewGuid();
        var settings = new UserSettings(userId, PreferredSex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        var user = new User(userId, "123456798", "test@test.com", "Janusz", new DateOnly(2000,1,1), UserSex.Male, null, settings);
        await _testDb.DbContext.Users.AddAsync(user);
        await _testDb.DbContext.SaveChangesAsync();

        var photo = new Photo(Guid.NewGuid(), userId, "path", "url", 1);
        await _testDb.DbContext.Photos.AddAsync(photo);
        await _testDb.DbContext.SaveChangesAsync();

        return photo;
    }

    private async Task<FileDto> CreateFileAsync(Guid photoId)
    {
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        var file = new FileDto
        {
            Id = photoId,
            Extension = "txt",
            Binary = data
        };

        await _testDb.DbContext.Files.AddAsync(file);
        await _testDb.DbContext.SaveChangesAsync();

        return file;
    }

    private readonly TestDatabase _testDb;
    public FileRepositoryTests()
    {
        _testDb = new TestDatabase();
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}