using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.PhotoManagement;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class PhotoServiceTests
{
    [Fact]
    public void file_should_be_saved_to_storage()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = "./";
        var storage = new FileStorage(storageOptions);
        byte[] photo = new byte[] {0x74, 0x65, 0x73, 0x74};
        var path = await storage.SaveFileAsync(photo, "test", "txt");
        var fileExists = System.IO.File.Exists(path);
        Assert.True(fileExists);
        System.IO.File.Delete(path);
    }

    [Fact]
    public void existing_file_should_be_deleted_from_storage()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = "./";
        var storage = new FileStorage(storageOptions);

        var path = System.IO.Path.Combine("./", "test", "txt");
        System.Io.File.Create(path);

        await storage.DeleteFileAsync(path);

        var fileExists = system.IO.File.Exists(path);
        Assert.False(fileExists);
    }

    [Fact]
    public void when_no_existing_file_delete_file_should_not_throw_exception()
    {
        IOptions<StorageOptions> storageOptions = Options.Create<StorageOptions>(new StorageOptions());
        storageOptions.Value.StoragePath = "./";
        var storage = new FileStorage(storageOptions);

        var path = System.IO.Path.Combine("./", "test", "txt");
        var exception = await Record.ExceptionAsync(async () => await storage.DeleteFileAsync(path));

        Assert.Null(exception);
    }

    [Fact]
    public void directory_is_created_if_needed_upon_save_photo()
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