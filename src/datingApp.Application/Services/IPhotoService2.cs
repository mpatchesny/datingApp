using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IPhotoService2<T> where T: class
{
    public bool ValidateSize(T content);
    public bool ValidateExtension(T content, out string extension);
}