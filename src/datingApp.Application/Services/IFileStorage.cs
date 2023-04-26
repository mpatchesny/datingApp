using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFIleStorage
{
    public string SavePhotoAsync(byte[] photo, string filename, string extension);
    public void DeletePhotoAsync(string path);
}