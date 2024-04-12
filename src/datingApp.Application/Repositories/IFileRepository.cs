using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Repositories;

public interface IFileRepository
{
    public Task<bool> ExistsAsync(string fileId);
    public Task SaveFileAsync(byte[] photo, string fileId, string extension);
    public Task<byte[]> GetByIdAsync(string fileId);
    public Task DeleteAsync(string fileId);
}