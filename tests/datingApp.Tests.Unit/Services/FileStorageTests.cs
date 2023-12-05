using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.PhotoManagement;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class FileStorageTests : IDisposable
{
    [Fact]
    public async Task file_should_be_saved_to_storageAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new DiskFileStorage(storageOptions, _logger);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        
        await storage.SaveFileAsync(photo, "test", "txt");
        
        string path = System.IO.Path.Combine(storageOptions.Value.StoragePath,"test.txt");
        var fileExists = System.IO.File.Exists(path);
        Assert.True(fileExists);
        System.IO.File.Delete(path);
    }

    [Fact]
    public async Task existing_file_should_be_deleted_from_storageAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new DiskFileStorage(storageOptions, _logger);

        var path = System.IO.Path.Combine(".", "test.txt");
        var stream = System.IO.File.Create(path);
        stream.Dispose();

        await storage.DeleteFileAsync("test");

        var fileExists = System.IO.File.Exists(path);
        Assert.False(fileExists);
    }

    [Fact]
    public async Task when_no_existing_file_delete_file_should_not_throw_exceptionAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new DiskFileStorage(storageOptions, _logger);
        
        var exception = await Record.ExceptionAsync(async () => await storage.DeleteFileAsync("test.txt"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task directory_is_created_if_needed_upon_save_photoAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = testDirectoryPath;
        var storage = new DiskFileStorage(storageOptions, _logger);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        
        await storage.SaveFileAsync(photo, "test", "txt");
        
        var folderExists = System.IO.Directory.Exists(testDirectoryPath);
        Assert.True(folderExists);
        System.IO.Directory.Delete(testDirectoryPath, true);
    }

    [Fact]
    public async Task if_file_not_exists_GetFileAsync_returns_null()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = testDirectoryPath;
        var storage = new DiskFileStorage(storageOptions, _logger);
        var file = await storage.GetFileAsync("some identification");
        Assert.Null(file);
    }

    [Fact]
    public async Task if_file_exists_GetFileAsync_returns_file_binary()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = testDirectoryPath;
        var storage = new DiskFileStorage(storageOptions, _logger);
        string filePath = System.IO.Path.Combine(storageOptions.Value.StoragePath, "identif.txt");
        await System.IO.File.WriteAllTextAsync(filePath, "abrakadabra");
        var file = await storage.GetFileAsync("identif");
        Assert.NotNull(file);
    }

    private string testDirectoryPath = "./test/";
    private readonly ILogger<IFileStorage> _logger;
    public FileStorageTests()
    {
        if (!System.IO.Directory.Exists(testDirectoryPath))
        {
            System.IO.Directory.CreateDirectory(testDirectoryPath);
        }

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        _logger = factory.CreateLogger<IFileStorage>();
    }

    public void Dispose()
    {
        if (System.IO.Directory.Exists(testDirectoryPath))
        {
            System.IO.Directory.Delete(testDirectoryPath, true);
        }
    }
}