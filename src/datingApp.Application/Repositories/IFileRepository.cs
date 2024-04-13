using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Repositories;

public interface IFileRepository
{
    public Task<bool> ExistsAsync(Guid fileId);
    public Task SaveFileAsync(byte[] photo, Guid fileId, string extension);
    public Task<byte[]> GetByIdAsync(Guid fileId);
    public Task DeleteAsync(Guid fileId);
}