using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Abstractions;

public interface IPhotoService
{
    public string SavePhoto(byte[] photo);
}