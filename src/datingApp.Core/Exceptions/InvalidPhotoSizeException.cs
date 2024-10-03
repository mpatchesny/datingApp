using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class InvalidPhotoSizeException : CustomException
{
    public InvalidPhotoSizeException(int minPhotoSizeKB, int maxPhotoSizeMB) : base($"Invalid photo size. Photo size should be between {minPhotoSizeKB} KB and {maxPhotoSizeMB} MB.")
    {
    }
}