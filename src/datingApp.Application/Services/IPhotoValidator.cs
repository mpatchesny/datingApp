using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IPhotoValidator
{
    public void Validate(Stream stream, out string extension);
    public void Validate(IFormFile file, out string extension);
}