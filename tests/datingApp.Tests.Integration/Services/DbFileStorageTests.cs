using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;
using Xunit;

namespace datingApp.Tests.Integration.Services;


public class DbFileStorageTests : IDisposable
{
    [Fact]
    public async Task given_file_exists_GetFileAsync_returns_file_binaryAsync()
    {
        byte[] data = new byte[3];
        data[0] = byte.MinValue;
        data[1] = 0;
        data[2] = byte.MaxValue;

        var file = new Application.DTO.FileDto {
            Id = "identif",
            Extension = "txt",
            Binary = data
        };
        _testDb.DbContext.Files.Add(file);
        await _testDb.DbContext.SaveChangesAsync();

        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        var fileBinary = await storage.GetByIdAsync("identif");
        Assert.NotNull(fileBinary);
        Assert.Same(fileBinary, data);
    }

    [Fact]
    public async Task given_file_not_exists_GetFileAsync_returns_null()
    {
        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        var file = await storage.GetByIdAsync("identif");
        Assert.Null(file);
    }

    [Fact]
    public async Task given_file_not_exists_SaveFileAsync_saves_file()
    {
        byte[] data = new byte[3];
        data[0] = byte.MinValue;
        data[1] = 0;
        data[2] = byte.MaxValue;

        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.SaveFileAsync(data, "identif" , "txt"));
        Assert.Null(exception);
        var file = await storage.GetByIdAsync("identif");
        Assert.NotNull(file);
    }

    [Fact]
    public async Task given_file_exists_SaveFileAsync_updates_file()
    {
        byte[] data = new byte[3];
        data[0] = byte.MinValue;
        data[1] = 0;
        data[2] = byte.MaxValue;

        var file = new Application.DTO.FileDto {
            Id = "identif",
            Extension = "txt",
            Binary = data
        };
        _testDb.DbContext.Files.Add(file);
        await _testDb.DbContext.SaveChangesAsync();

        data[0] = byte.MaxValue;
        data[1] = byte.MaxValue;
        data[2] = byte.MaxValue;

        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.SaveFileAsync(data, "identif" , "txt"));
        Assert.Null(exception);
        var fileBinary = await storage.GetByIdAsync("identif");
        Assert.NotNull(fileBinary);
        Assert.Same(data, fileBinary);
    }

    [Fact]
    public async Task given_file_exists_DeleteFileAsync_deletes_files()
    {
        byte[] data = new byte[3];
        data[0] = byte.MinValue;
        data[1] = 0;
        data[2] = byte.MaxValue;

        var file = new Application.DTO.FileDto {
            Id = "identif",
            Extension = "txt",
            Binary = data
        };
        _testDb.DbContext.Files.Add(file);
        await _testDb.DbContext.SaveChangesAsync();

        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        await storage.DeleteAsync("identif");
        file = _testDb.DbContext.Files.FirstOrDefault(x => x.Id == "identif");
        Assert.Null(file);
    }

    [Fact]
    public async Task given_file_notexists_DeleteFileAsync_not_throws_exepction()
    {
        IFileRepository storage = new PostgresFileRepository(_testDb.DbContext);
        var exception = await Record.ExceptionAsync(async () => await storage.DeleteAsync("identif"));
        Assert.Null(exception);
    }

    // Arrange
    private readonly TestDatabase _testDb;
    public DbFileStorageTests()
    {
        _testDb = new TestDatabase();
    }

    // Teardown
    public void Dispose()
    {
        _testDb.Dispose();
    }
}