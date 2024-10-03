using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class InvalidPhotoSizeExceptionCore : CustomException
{
    public InvalidPhotoSizeExceptionCore(int minPhotoSizeKB, int maxPhotoSizeMB) : base($"Invalid photo size. Photo size should be between {minPhotoSizeKB} KB and {maxPhotoSizeMB} MB.")
    {
    }
}