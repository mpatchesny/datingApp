using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentStorage;
using FluentStorage.Blobs;

namespace datingApp.Tests.Unit.Mocks;

internal class MockFileStorageService : IBlobStorage
{
    public List<string> DeletedItems = new List<string>();

    public Task DeleteAsync(IEnumerable<string> fullPaths, CancellationToken cancellationToken = default)
    {
        foreach (var item in fullPaths)
        {
            DeletedItems.Add(item);
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<bool>> ExistsAsync(IEnumerable<string> fullPaths, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Blob>> GetBlobsAsync(IEnumerable<string> fullPaths, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Blob>> ListAsync(ListOptions options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> OpenReadAsync(string fullPath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ITransaction> OpenTransactionAsync()
    {
        throw new NotImplementedException();
    }

    public Task SetBlobsAsync(IEnumerable<Blob> blobs, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(string fullPath, Stream dataStream, bool append = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    Task<Stream> IBlobStorage.OpenReadAsync(string fullPath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<ITransaction> IBlobStorage.OpenTransactionAsync()
    {
        throw new NotImplementedException();
    }
}