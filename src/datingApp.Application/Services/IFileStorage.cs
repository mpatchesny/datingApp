using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileStorage
{
    public Task <string> SaveFileAsync(byte[] photo, string filename, string extension);
    public Task DeleteFileAsync(string path);
}