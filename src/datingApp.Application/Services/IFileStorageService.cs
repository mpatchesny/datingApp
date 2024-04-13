using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileStorageService
{
    public bool Exists(string fileId, string extension);
    public void SaveFile(byte[] file, string fileId, string extension);
    public byte[] GetFile(string fileId, string extension);
    public void DeleteFile(string fileId);
    public void DeleteFile(string fileId, string extension);
}