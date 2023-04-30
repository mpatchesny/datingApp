using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileStorage
{
    public Task SaveFileAsync(byte[] photo, string identification, string extension);
    public Task DeleteFileAsync(string identification);
}