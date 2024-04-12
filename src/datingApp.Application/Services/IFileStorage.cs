using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileStorage
{
    public Task<bool> ExistsAsync(string identification);
    public Task SaveFileAsync(byte[] photo, string identification, string extension);
    public Task<byte[]> GetFileAsync(string identification);
    public Task DeleteFileAsync(string identification);
}