using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.PhotoManagement;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class FileStorageTests
{
    [Fact]
    public async Task file_should_be_saved_to_storageAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new FileStorage(storageOptions);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var path = await storage.SaveFileAsync(photo, "test", "txt");
        var fileExists = System.IO.File.Exists(path);
        Assert.True(fileExists);
        System.IO.File.Delete(path);
    }

    [Fact]
    public async Task existing_file_should_be_deleted_from_storageAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new FileStorage(storageOptions);

        var path = System.IO.Path.Combine(".", "test.txt");
        var stream = System.IO.File.Create(path);
        stream.Dispose();

        await storage.DeleteFileAsync(path);

        var fileExists = System.IO.File.Exists(path);
        Assert.False(fileExists);
    }

    [Fact]
    public async Task when_no_existing_file_delete_file_should_not_throw_exceptionAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = ".";
        var storage = new FileStorage(storageOptions);

        var path = System.IO.Path.Combine(".", "test", "txt");
        var exception = await Record.ExceptionAsync(async () => await storage.DeleteFileAsync(path));

        Assert.Null(exception);
    }

    [Fact]
    public async Task directory_is_created_if_needed_upon_save_photoAsync()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = testDirectoryPath;
        var storage = new FileStorage(storageOptions);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var path = await storage.SaveFileAsync(photo, "test", "txt");
        var folderExists = System.IO.Directory.Exists(testDirectoryPath);
        Assert.True(folderExists);
        System.IO.File.Delete(path);
        System.IO.Directory.Delete(testDirectoryPath);
    }

    private string testDirectoryPath = "./test/";
    public FileStorageTests()
    {
        if (System.IO.Directory.Exists(testDirectoryPath))
        {
            System.IO.Directory.Delete(testDirectoryPath);
        }
        
    }
}