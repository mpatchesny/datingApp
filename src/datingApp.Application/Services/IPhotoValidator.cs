using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IPhotoValidator<T> where T: class
{
    public void ValidateSize(T content);
    public void ValidateExtension(T content, out string extension);
}