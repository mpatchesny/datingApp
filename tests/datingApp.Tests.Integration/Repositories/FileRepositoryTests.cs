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
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication;
using Xunit;

namespace datingApp.Tests.Integration.Repositories;


public class FileRepositoryTests : IDisposable
{
    [Fact]
    public async Task given_file_exists_ExistsAsync_returns_true()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        _ = await CreateFileAsync(photo.Id);

        var exists = await _storage.ExistsAsync(photo.Id);
        Assert.True(exists);
    }

    [Fact]
    public async Task given_file_not_exists_ExistsAsync_returns_false()
    {
        var exists = await _storage.ExistsAsync(Guid.NewGuid());
        Assert.False(exists);
    }

    [Fact]
    public async Task given_file_exists_GetFileAsync_returns_file_binary()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        var file = await CreateFileAsync(photo.Id);

        var fileBinary = await _storage.GetByIdAsync(file.Id);
        Assert.Same(fileBinary, file.Binary);
    }

    [Fact]
    public async Task given_file_not_exists_GetFileAsync_returns_null()
    {
        var file = await _storage.GetByIdAsync(Guid.NewGuid());
        Assert.Null(file);
    }

    [Fact]
    public async Task given_file_not_exists_SaveFileAsync_saves_file()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        var fileId = photo.Id;

        var exception = await Record.ExceptionAsync(async () => await _storage.AddAsync(data, fileId , "txt"));
        Assert.Null(exception);

        var file = await _testDb.DbContext.Files.FirstOrDefaultAsync(x => x.Id == photo.Id);
        Assert.NotNull(file);
    }

    [Fact]
    public async Task given_file_exists_SaveFileAsync_updates_file()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        var file = await CreateFileAsync(photo.Id);

        file.Binary[0] = byte.MaxValue;
        file.Binary[1] = byte.MaxValue;
        file.Binary[2] = byte.MaxValue;

        var exception = await Record.ExceptionAsync(async () => await _storage.AddAsync(file.Binary, file.Id , "txt"));
        Assert.Null(exception);

        var fileBinary = (await _testDb.DbContext.Files.FirstOrDefaultAsync(x => x.Id == photo.Id))?.Binary;
        Assert.Same(file.Binary, fileBinary);
    }

    [Fact]
    public async Task given_file_exists_DeleteFileAsync_deletes_files()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        var file = await CreateFileAsync(photo.Id);
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };

        await _storage.DeleteAsync(file.Id);

        var exists = await _testDb.DbContext.Files.AnyAsync(x => x.Id == photo.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task given_file_not_exists_DeleteFileAsync_not_throws_exepction()
    {
        var exception = await Record.ExceptionAsync(async () => await _storage.DeleteAsync(Guid.NewGuid()));
        Assert.Null(exception);
    }

    // Arrange
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
    private readonly IFileRepository _storage;
    public FileRepositoryTests()
    {
        _testDb = new TestDatabase();
        var compressor = new DummyFileCompressor();
        _storage = new DbFileRepository(_testDb.DbContext, compressor);
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}