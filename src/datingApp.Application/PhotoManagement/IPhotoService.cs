using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.PhotoManagement;

public interface IPhotoService
{
    public byte[] ConvertToArrayOfBytes(string base64content);
    public void ValidatePhoto(byte[] photo);
    public string GetImageFileFormat(byte[] photo);
}