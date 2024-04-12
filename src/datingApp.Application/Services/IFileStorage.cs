using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileStorage
{
    public Task<bool> ExistsAsync(string fileId);
    public Task SaveFileAsync(byte[] photo, string fileId, string extension);
    public Task<byte[]> GetFileAsync(string fileId);
    public Task DeleteFileAsync(string fileId);
}