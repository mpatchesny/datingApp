using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class FileStorageServiceTests : IDisposable
{
    [Fact]
    public void given_file_exists_exists_returns_true()
    {
        var filePath = System.IO.Path.Combine(_storagePath, "test.txt");
        System.IO.File.Create(filePath);

        var result = _storageService.Exists("test", "txt");
        Assert.True(result);
    }

    [Fact]
    public void given_file_not_exists_exists_returns_false()
    {
        var result = _storageService.Exists("test", "txt");
        Assert.False(result);
    }

    [Fact]
    public void given_file_exists_get_file_returns_file_content()
    {
        var filePath = System.IO.Path.Combine(_storagePath, "test.txt");
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        System.IO.File.WriteAllBytes(filePath, data);

        var result = _storageService.GetFile("test", "txt");
        Assert.Equal(data, result);
    }

    [Fact]
    public void given_valid_input_save_file_saves_file()
    {
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        _storageService.SaveFile(data, "test", "txt");

        var exists = _storageService.Exists("test", "txt");
        Assert.True(exists);
    }

    [Fact]
    public void given_invalid_input_save_not_throws_exception()
    {
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        var exception = Record.Exception(() => _storageService.SaveFile(data, "test/test", "txt"));
        Assert.Null(exception);

        var dir = new System.IO.DirectoryInfo(_storagePath);
        var files = dir.GetFiles();
        Assert.Empty(files);
    }

    [Fact]
    public void given_file_not_exists_get_file_returns_null()
    {
        var result = _storageService.GetFile("test", "txt");
        Assert.Null(result);
    }

    [Fact]
    public void given_file_exists_delete_file_by_file_id_and_extension_deletes_file()
    {
        var filePath = System.IO.Path.Combine(_storagePath, "test.txt");
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        System.IO.File.WriteAllBytes(filePath, data);

        _storageService.DeleteFile("test", "txt");
        var exists = _storageService.Exists("test", "txt");
        Assert.False(exists);
    }

    [Fact]
    public void given_file_exists_delete_file_by_file_id_and_extension_not_throws_exception()
    {
        var exception = Record.Exception(() => _storageService.DeleteFile("test", "txt"));
        Assert.Null(exception);

        var exists = _storageService.Exists("test", "txt");
        Assert.False(exists);
    }

    [Fact]
    public void given_file_exists_delete_file_by_file_id_deletes_file()
    {
        var filePath = System.IO.Path.Combine(_storagePath, "test.txt");
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        System.IO.File.WriteAllBytes(filePath, data);

        _storageService.DeleteFile("test");
        var exists = _storageService.Exists("test", "txt");
        Assert.False(exists);
    }

    [Fact]
    public void given_file_exists_delete_file_by_file_id_not_throws_exception()
    {
        var exception = Record.Exception(() => _storageService.DeleteFile("test"));
        Assert.Null(exception);

        var exists = _storageService.Exists("test", "txt");
        Assert.False(exists);
    }

    private readonly FileStorageService _storageService;
    private readonly string _storagePath;
    private readonly DirectoryInfo _dir;
    public FileStorageServiceTests()
    {
        _storagePath = System.IO.Path.Combine(
                Path.GetTempPath(), $"datingapptest_{Guid.NewGuid()}"
            );
        _dir = new DirectoryInfo(_storagePath);
        _dir.Create();

        var options = new StorageOptions { StoragePath = _storagePath };
        ILogger<FileStorageService> logger = new Logger<FileStorageService>(new LoggerFactory());
        _storageService = new FileStorageService(Options.Create<StorageOptions>(options), logger);
    }

    public async void Dispose()
    {
        await Task.Delay(1000);
        _dir.Delete(true);
    }
}