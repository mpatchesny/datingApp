using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

public interface IPhotoService
{
    PhotoServiceProcessOutput ProcessBase64Photo(string base64Bytes);
}

public sealed record PhotoServiceProcessOutput(byte[] Bytes, string Extension);