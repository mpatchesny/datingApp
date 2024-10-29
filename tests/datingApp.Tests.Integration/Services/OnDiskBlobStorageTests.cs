using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentStorage;
using FluentStorage.Blobs;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class OnDiskBlobStorageTests : IDisposable
{
    [Fact]
    public async void given_valid_input_WriteAsync_with_stream_saves_file()
    {
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        var filename = "test.txt";
        await _storageService.WriteAsync(filename, new MemoryStream(data));

        var filePath = System.IO.Path.Combine(_storagePath, filename);
        var exists = System.IO.File.Exists(filePath);
        Assert.True(exists);
    }

    [Fact]
    public async void given_file_exists_DeleteAsync_with_path_deletes_file()
    {
        var filename = "test.txt";
        var filePath = System.IO.Path.Combine(_storagePath, filename);
        byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
        System.IO.File.WriteAllBytes(filePath, data);

        await _storageService.DeleteAsync(filename);
        var exists = System.IO.File.Exists(filePath);
        Assert.False(exists);
    }

    [Fact]
    public async void given_file_exists_DeleteAsync_with_list_of_paths_deletes_list_of_files()
    {
        var fileNames = new List<string>();
        var filePaths = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var filename = $"test_{guid}.txt";
            var filePath = System.IO.Path.Combine(_storagePath, filename);
            byte[] data = new byte[] { byte.MinValue, 0, byte.MaxValue };
            System.IO.File.WriteAllBytes(filePath, data);
            fileNames.Append(filename);
            filePaths.Append(filePath);
        }

        await _storageService.DeleteAsync(fileNames);
        foreach (var file in filePaths)
        {
            var exists = System.IO.File.Exists(file);
            Assert.False(exists);
        }
    }

    [Fact]
    public async void given_file_not_exists_DeleteAsync_with_path_not_throws_exception()
    {
        var exception = await Record.ExceptionAsync(() => _storageService.DeleteAsync("not_existing_file.txt"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task given_files_not_exists_DeleteAsync_with_list_of_paths_not_throws_exception()
    {
        var notExistingFiles = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var filename = $"test_{guid}.txt";
            notExistingFiles.Append(filename);
        }

        var exception = await Record.ExceptionAsync(() => _storageService.DeleteAsync(notExistingFiles));
        Assert.Null(exception);
    }

    private readonly IBlobStorage _storageService;
    private readonly string _storagePath;
    public OnDiskBlobStorageTests()
    {
        _storagePath = System.IO.Path.Combine(
                Path.GetTempPath(), $"datingapptest_{Guid.NewGuid()}"
            );
        var dir = new DirectoryInfo(_storagePath);
        dir.Create();
        _storageService = StorageFactory.Blobs.DirectoryFiles(_storagePath);
    }

    public void Dispose()
    {
        Task.Delay(1000);
        var dir = new DirectoryInfo(_storagePath);
        dir.Delete(true);
    }
}